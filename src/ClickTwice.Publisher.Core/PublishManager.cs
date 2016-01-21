using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Tasks;

namespace ClickTwice.Publisher.Core
{
    public class PublishManager : Manager, IDisposable
    {
        private BuildManager Manager => BuildManager.DefaultBuildManager;
        private bool GenerateManifest { get; set; }
        private ManifestManager ManifestManager { get; set; }
        private InformationSource ManifestInformationSource { get; set; }

        public PublishManager(string projectFilePath, InformationSource manifestInformationSource) : base(projectFilePath)
        {
            switch (manifestInformationSource)
            {
                case InformationSource.AssemblyInfo:
                case InformationSource.AppManifest:
                case InformationSource.Both:
                    GenerateManifest = true;
                    break;
                case InformationSource.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(manifestInformationSource), manifestInformationSource, null);
            }
            ManifestInformationSource = manifestInformationSource;
        }

        public string Configuration { private get; set; } = "Release";
        public string Platform { private get; set; } = "x86";


        public List<IInputHandler> InputHandlers { private get; set; } = new List<IInputHandler>();
        public List<IOutputHandler> OutputHandlers { private get; set; } = new List<IOutputHandler>();

        /// <exception cref="HandlerProcessingException">Thrown when input or output handlers encounter an exception.</exception>
        /// <exception cref="OperationInProgressException">Thrown when a build or publish operation is already in progress.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid behaviour type provided.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="BuildFailedException">Thrown when the build fails.</exception>
        public List<HandlerResponse> PublishApp(string targetPath, PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var pc = new ProjectCollection();
            if (!File.Exists(ProjectFilePath))
            {
                throw new FileNotFoundException($"Project file not found at path {ProjectFilePath}! Please ensure you have provided a valid csproj file.", ProjectFilePath);
            }
            var loggers = new List<ILogger> {new ConsoleLogger(LoggerVerbosity.Normal)};
            var path = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString("N") + "\\");
            var props = new Dictionary<string, string> {{"Configuration", Configuration}, {"Platform", Platform}, {"OutputPath", path.FullName}};
            var results = ProcessInputHandlers();
            if (results.All(r => r.Result != HandlerResult.Error))
            {
                var targets = new List<string> {"PrepareForBuild"};
                switch (behaviour)
                {
                    case PublishBehaviour.None:
                        targets.Add("Build", "Publish");
                        break;
                    case PublishBehaviour.CleanFirst:
                        targets.Add("Clean", "Build", "Publish");
                        break;
                    case PublishBehaviour.DoNotBuild:
                        targets.Add("Publish");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
                }
                BuildParameters buildParams = new BuildParameters(pc)
                {
                    DetailedSummary = true, Loggers = loggers, DefaultToolsVersion = "14.0"
                };
                var reqData = new BuildRequestData(ProjectFilePath, props, null, targets.ToArray(), null);
                try
                {
                    Manager.BeginBuild(buildParams);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("in progress"))
                {
                    throw new OperationInProgressException(OperationType.Build, ex);
                }
                var buildResult = Manager.BuildRequest(reqData);
                List<HandlerResponse> output;
                if (buildResult.OverallResult == BuildResultCode.Success)
                {
                    var publishDir = path.GetDirectories().FirstOrDefault(d => d.Name == "app.publish");
                    if (GenerateManifest)
                    {
                        PrepareManifestManager(publishDir?.FullName);
                        ManifestManager.DeployManifest(ManifestManager.CreateAppManifest());
                    }
                    output = ProcessOutputHandlers(publishDir);
                    if (output.Any(o => o.Result == HandlerResult.Error))
                    {
                        throw new HandlerProcessingException(OutputHandlers, output);
                    }
                    if (!string.IsNullOrWhiteSpace(targetPath))
                    {
                        publishDir.Copy(destDirPath: targetPath, copySubDirs: true);
                    }
                }
                else
                {
                    throw new BuildFailedException(buildResult.Exception, buildResult.ResultsByTarget.Values.Where(t => t?.Exception != null).Select(r => r.Exception));
                }
                if (CleanOutputOnCompletion)
                {
                    Directory.Delete(path.FullName, true);
                }
                return output;
            }
            else
            {
                throw new HandlerProcessingException(InputHandlers, results);
            }
        }

        private void PrepareManifestManager(string targetPath)
        {
            if (!string.IsNullOrWhiteSpace(targetPath) && GenerateManifest)
            {
                ManifestManager = new ManifestManager(ProjectFilePath, targetPath, ManifestInformationSource);
            }
        }

        private List<HandlerResponse> ProcessInputHandlers()
        {
            var fi = new FileInfo(ProjectFilePath).Directory?.FullName;
            var list = new List<HandlerResponse>();
            if (string.IsNullOrWhiteSpace(fi))
            {
                return null;
            }
            foreach (var handler in InputHandlers)
            {
                try
                {
                    var result = handler.Process(fi);
                    if (result.Result == HandlerResult.NotRun)
                    {
                        result.Result = HandlerResult.OK;
                    }
                    list.Add(result);
                }
                catch (Exception ex)
                {
                    list.Add(new HandlerResponse(handler, false, ex.Message));
                }
            }
            return list;
        }

        private List<HandlerResponse> ProcessOutputHandlers(FileSystemInfo outputPath)
        {
            var results = OutputHandlers.Where(_ => !string.IsNullOrWhiteSpace(outputPath?.FullName)).Select(handler => handler.Process(outputPath.FullName)).ToList();
            return results;
        }

        public void Dispose()
        {
            Manager.EndBuild();
            Manager.Dispose();
        }
    }

    public enum OperationType
    {
        Clean,
        Build,
        Publish,
        Deploy
    }

    public enum PublishBehaviour
    {
        None,
        CleanFirst,
        DoNotBuild
    }
}
