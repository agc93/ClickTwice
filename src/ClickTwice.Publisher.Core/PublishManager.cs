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

        public PublishManager(string projectFilePath) : base(projectFilePath)
        {
            
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
            var loggers = new List<ILogger> {new ConsoleLogger(LoggerVerbosity.Normal)};
            var path = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString("N") + "\\");
            var props = new Dictionary<string, string> {{"Configuration", Configuration}, {"Platform", Platform}, {"OutputPath", path.FullName} };
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
                    DetailedSummary = true,
                    Loggers = loggers,
                    DefaultToolsVersion = "14.0"
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
                List<HandlerResponse> output = new List<HandlerResponse>();
                if (buildResult.OverallResult == BuildResultCode.Success)
                {
                    output = ProcessOutputHandlers(path);
                    if (output.Any(o => o.Result == HandlerResult.Error))
                    {
                        throw new HandlerProcessingException(OutputHandlers, output);
                    }
                    if (!string.IsNullOrWhiteSpace(targetPath))
                    {
                        path.Copy(destDirPath: targetPath, copySubDirs: true); 
                    }
                }
                else
                {
                    throw new BuildFailedException(buildResult.Exception,
                        buildResult.ResultsByTarget.Values.Where(t => t?.Exception != null).Select(r => r.Exception));
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
