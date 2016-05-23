using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Tasks;
using ConsoleLogger = Microsoft.Build.Logging.ConsoleLogger;

namespace ClickTwice.Publisher.Core
{
    public class PublishManager : Manager, IDisposable
    {
        private BuildManager Manager => BuildManager.DefaultBuildManager;
        private bool GenerateManifest { get; set; }
        private ManifestManager ManifestManager { get; set; }
        private InformationSource ManifestInformationSource { get; set; }

        public PublishManager(string projectFilePath, InformationSource manifestInformationSource)
            : base(projectFilePath)
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
                    throw new ArgumentOutOfRangeException(nameof(manifestInformationSource), manifestInformationSource,
                        null);
            }
            ManifestInformationSource = manifestInformationSource;
        }

        public string Configuration { private get; set; } = "Release";
        public string Platform { private get; set; } = "x86";


        public List<IInputHandler> InputHandlers { private get; set; } = new List<IInputHandler>();
        public List<IOutputHandler> OutputHandlers { private get; set; } = new List<IOutputHandler>();
        public List<IBuildConfigurator> BuildConfigurators { private get; set; } = new List<IBuildConfigurator>();
        public List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();

        /// <exception cref="HandlerProcessingException">Thrown when input or output handlers encounter an exception.</exception>
        /// <exception cref="OperationInProgressException">Thrown when a build or publish operation is already in progress.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid behaviour type provided.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="BuildFailedException">Thrown when the build fails.</exception>
        public List<HandlerResponse> PublishApp(string targetPath,
            PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var pc = new ProjectCollection();
            if (!File.Exists(ProjectFilePath))
            {
                throw new FileNotFoundException(
                    $"Project file not found at path {ProjectFilePath}! Please ensure you have provided a valid csproj file.",
                    ProjectFilePath);
            }
            Log("Configuring loggers");
            var loggers = new List<ILogger> {new BuildMessageLogger(Loggers.Where(l => l.IncludeBuildMessages).ToList()) };
            Log($"Added {loggers.Count} loggers");
            Log("Configuring build environment");
            var path = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString("N") + "\\");
            var props = new Dictionary<string, string>
            {
                {"Configuration", Configuration},
                {"Platform", Platform},
                {"OutputPath", path.FullName}
            };
            Log("Processing input handlers");
            var results = ProcessInputHandlers();
            Log($"Completed processing input handlers: {results.Count(r => r.Result == HandlerResult.OK)} OK, {results.Count(r => r.Result == HandlerResult.Error)} errors, {results.Count(r => r.Result == HandlerResult.NotRun)} not run");
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
                Log("Running additional configurators");
                var configurators = RunConfigurators(props, targets);
                props = configurators.Key;
                targets = configurators.Value;
                Log($"Build configured for targets: {string.Join(", ", targets)}");
                var buildParams = new BuildParameters(pc)
                {
                    DetailedSummary = true,
                    Loggers = loggers,
                    DefaultToolsVersion = "14.0"
                };
                var reqData = new BuildRequestData(ProjectFilePath, props, "14.0", targets.ToArray(), null);
                Log("Preparing for build");
                try
                {
                    Manager.BeginBuild(buildParams);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("in progress"))
                {
                    throw new OperationInProgressException(OperationType.Build, ex);
                }
                Log("Starting MSBuild build");
                var buildResult = Manager.BuildRequest(reqData);
                Log($"MSBuild build complete: {buildResult.OverallResult}");
                List<HandlerResponse> output;
                if (buildResult.OverallResult == BuildResultCode.Success)
                {
                    var publishDir = path.GetDirectories().FirstOrDefault(d => d.Name == "app.publish");
                    if (GenerateManifest)
                    {
                        Log("Generating ClickTwice manifest");
                        PrepareManifestManager(publishDir?.FullName);
                        ManifestManager.DeployManifest(ManifestManager.CreateAppManifest());
                    }
                    Log("Processing output handlers");
                    output = ProcessOutputHandlers(publishDir);
                    Log(
                        $"Completed processing output handlers: {output.Count(r => r.Result == HandlerResult.OK)} OK, {output.Count(r => r.Result == HandlerResult.Error)} errors, {output.Count(r => r.Result == HandlerResult.NotRun)} not run");
                    if (output.Any(o => o.Result == HandlerResult.Error))
                    {
                        Log("Error encountered while processing output handlers. Aborting!");
                        throw new HandlerProcessingException(OutputHandlers, output);
                    }
                    if (!string.IsNullOrWhiteSpace(targetPath))
                    {
                        Log("Copying publish results to target directory");
                        publishDir.Copy(destDirPath: targetPath, copySubDirs: true);
                    }
                }
                else
                {
                    Log(
                        $"MSBuild build failed with {buildResult.Exception.GetType().Name}: {buildResult.Exception.Message}");
                    throw new BuildFailedException(buildResult.Exception,
                        buildResult.ResultsByTarget.Values.Where(t => t?.Exception != null).Select(r => r.Exception));
                }
                if (!CleanOutputOnCompletion) return output;
                CloseLoggers(targetPath);
                Log("Cleaning build output directory");
                Directory.Delete(path.FullName, true);
                return output;
            }
            else
            {
                Log("Error encountered while processing input handlers. Aborting!");
                throw new HandlerProcessingException(InputHandlers, results);
            }
        }

        private void CloseLoggers(string targetPath)
        {
            var path = Path.Combine(targetPath, "Logs");
            Directory.CreateDirectory(path);
            Log("Closing loggers");
            foreach (var logger in Loggers)
            {
                try
                {
                    var result = logger.Close(path);
                    Log($"Logger result: {result}");
                }
                catch (NotImplementedException)
                {
                    // ignored
                }
                catch (Exception ex)
                {
                    Log($"Exception encountered while closing logger: {ex.Message}");
                }
            }
        }

        private KeyValuePair<Dictionary<string, string>, List<string>> RunConfigurators(Dictionary<string, string> props, List<string> targetList)
        {
            var properties = props;
            var targets = targetList;
            foreach (var configurator in BuildConfigurators)
            {
                //var loggers = configurator.AddLoggers();
                //Loggers.AddRange(loggers.Where(l => l != null && !Loggers.Contains(l)));
                //foreach (var logger in loggers.Where(l => Loggers.Contains(l)))
                //{
                //    Loggers[Loggers.IndexOf(logger)].IncludeBuildMessages = true;
                //}
                try
                {
                    var o = configurator.ProcessConfiguration(props);
                    if ((o != null) && (o != props) && (o.Any()))
                    {
                        properties = o;
                    }
                    var t = configurator.ProcessTargets(targets);
                    if ((t != null) && (t != targets) && (t.Any()))
                    {
                        targets = t;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return new KeyValuePair<Dictionary<string, string>, List<string>>(properties, targets);
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
                    Log($"Handler {handler.Name} returned {result.Result}: {result.ResultMessage}");
                }
                catch (Exception ex)
                {
                    list.Add(new HandlerResponse(handler, false, ex.Message));
                    Log($"Handler {handler.Name} encountered {ex.GetType().Name}: {ex.Message}");
                }
            }
            return list;
        }

        private void Log(string content, bool isBuildMessage = false)
        {
            foreach (var logger in Loggers.Where(l => isBuildMessage ? l.IncludeBuildMessages : l != null))
            {
                logger.Log($"{DateTime.Now.ToString("T")} - {content}");
            }
        }

        private List<HandlerResponse> ProcessOutputHandlers(FileSystemInfo outputPath)
        {
            var results =
                OutputHandlers.Where(_ => !string.IsNullOrWhiteSpace(outputPath?.FullName))
                    .Select(handler => handler.Process(outputPath.FullName))
                    .ToList();
            return results;
        }

        /// <summary>
        /// Reads the assembly version number from AssemblyInfo.cs and updates it in the project file
        /// </summary>
        public void SyncVersionNumber()
        {
            
            var v = ReadVersionFromAssemblyInfo();
            SaveVersionToProjectFile(v);
        }

        private void SaveVersionToProjectFile(string v)
        {
            var doc = XDocument.Load(ProjectFilePath);
            var vElement = doc.XPathSelectElement(
                "//*[local-name()='Project']/*[local-name()='Property-Group']/*[local-name()='ApplicationVersion']");
            vElement.Value = v;
            doc.Save(ProjectFilePath);
        }

        private string ReadVersionFromAssemblyInfo()
        {
            var projectFolder = new FileInfo(ProjectFilePath).Directory;
            var infoFilePath = Path.Combine(projectFolder.FullName, "Properties", "AssemblyInfo.cs");
            var props = File.ReadAllLines(infoFilePath).Where(l => l.StartsWith("[assembly: ")).ToList();
            var v = props.Property("Version");
            return v;
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