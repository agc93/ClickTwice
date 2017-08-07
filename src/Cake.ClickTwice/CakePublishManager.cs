using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using Path = System.IO.Path;

namespace Cake.ClickTwice
{
    /// <summary>
    /// ClickTwice publish host for Cake
    /// </summary>
    internal class CakePublishManager : Manager, IPublishManager
    {
        public CakePublishManager(ClickTwiceManager mgr) : base(mgr.ProjectFilePath)
        {
            Environment = mgr.Environment;
            FileSystem = mgr.FileSystem;
            Runner = mgr.ProcessRunner;
            ToolLocator = mgr.ToolLocator;
            BuildAction = mgr.BuildAction ?? DefaultBuildAction;
            CleanOutputOnCompletion = mgr.CleanOutput;
            Configuration = mgr.Configuration;
            Platform = mgr.Platform;
            InputHandlers = mgr.InputHandlers;
            OutputHandlers = mgr.OutputHandlers;
            Loggers.AddRange(mgr.Loggers);
            ErrorAction = mgr.ErrorAction;
            if (!string.IsNullOrWhiteSpace(mgr.PublishVersion))
                AdditionalProperties.Add("ApplicationVersion", mgr.PublishVersion.ToVersionString());
        }

        private Action<IEnumerable<HandlerResponse>> ErrorAction { get; set; }

        private Action<CakePublishManager> BuildAction { get; set; }

        public void Dispose()
        {
        }

        /// <summary>
        /// Build configuration to use
        /// </summary>
        public string Configuration { private get; set; }

        /// <summary>
        /// Build platform to use
        /// </summary>
        public string Platform { private get; set; }

        private bool GenerateManifest { get; set; } = true;

        /// <summary>
        /// Collection of ClickTwice input handlers to run before publishing
        /// </summary>
        public List<IInputHandler> InputHandlers { private get; set; }

        /// <summary>
        /// Collection of ClickTwice output handlers to run after publishing
        /// </summary>
        public List<IOutputHandler> OutputHandlers { get; set; }

        /// <summary>
        /// Collection of build configurators to update the build configuration
        /// </summary>
        public List<IBuildConfigurator> BuildConfigurators { get; set; }

        /// <summary>
        /// Collection of ClickTwice loggers to log messages to
        /// </summary>
        public List<IPublishLogger> Loggers { get; } = new List<IPublishLogger>();

        private ManifestManager ManifestManager { get; set; }

        /// <summary>
        /// Publishes the app to the given folder
        /// </summary>
        /// <param name="targetPath">Folder to publsh the project too</param>
        /// <param name="behaviour">Preferred treatment of previous builds</param>
        /// <returns>The collection of results from the output handlers</returns>
        public List<HandlerResponse> PublishApp(string targetPath,
            PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var results = InputHandlers.ProcessHandlers(
                new FilePath(ProjectFilePath).GetDirectory().MakeAbsolute(Environment).FullPath, s => Log(s));
            Log(
                $"Completed processing input handlers: {results.Count(r => r.Result == HandlerResult.OK)} OK, {results.Count(r => r.Result == HandlerResult.Error)} errors, {results.Count(r => r.Result == HandlerResult.NotRun)} not run");
            if (results.Any(r => r.Result == HandlerResult.Error))
                throw new HandlerProcessingException(InputHandlers, results);
            string outputPath;
            if (behaviour == PublishBehaviour.DoNotBuild)
            {
                outputPath = Path.Combine(new FilePath(ProjectFilePath).GetDirectoryPath(Environment), "bin", Configuration);
                BuildAction = null;
            }
            else
            {
                outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                if (!FileSystem.Exist((DirectoryPath) outputPath)) FileSystem.GetDirectory(outputPath).Create();
            }
            var props = new Dictionary<string, string>
            {
                {"Configuration", Configuration},
                {"Platform", Platform},
                {"OutputPath", outputPath},
                {"PublishDir", Path.Combine(outputPath, "app.publish") + "\\"}
            };
            BuildSettings = new MSBuildSettings
            {
                Configuration = Configuration,
                MSBuildPlatform = GetMSBuildPlatform(Platform),
                Verbosity = Verbosity.Quiet
            };
            BuildSettings.AddTargets(behaviour);
            BuildSettings.AddProperties(props, AdditionalProperties);
            Log($"Invoking build actions for {ProjectFilePath} ({behaviour})");
            BuildAction?.Invoke(this);
            var publishDir = new DirectoryPath(props["PublishDir"]); // much simpler. Stupider, but simpler.
            /*var publishDir =
                new DirectoryPath(
                    new DirectoryInfo(props["OutputPath"]).GetDirectories()
                        .FirstOrDefault(d => d.Name == "app.publish")?
                        .FullName); */
            // the above logic falls apart fast when the directory doesn't exist yet (i.e. for DoNotBuild behaviour)
            Log(FileSystem.Exist(publishDir)
                ? "No publish directory found in app directory (using 'app.publish')"
                : $"Found publish directory at {publishDir.GetDirectoryName()}");
            if (GenerateManifest)
            {
                PrepareManifestManager(publishDir, InformationSource.Both);
                ManifestManager.DeployManifest(ManifestManager.CreateAppManifest());
            }
            Log("Processing output handlers");
            var output = OutputHandlers.ProcessHandlers(publishDir.FullPath, s => Log(s));
            Log(
                $"Completed processing output handlers: {output.Count(r => r.Result == HandlerResult.OK)} OK, {output.Count(r => r.Result == HandlerResult.Error)} errors, {output.Count(r => r.Result == HandlerResult.NotRun)} not run");
            if (output.Any(o => o.Result == HandlerResult.Error) && ErrorAction != null)
            {
                Log("Error encountered while processing output handlers. Aborting!");
                ErrorAction?.Invoke(output);
                //throw new HandlerProcessingException(OutputHandlers, output); // in case something goes real wrong
            }
            if (string.IsNullOrWhiteSpace(targetPath)) return output;
            Log("Copying publish results to target directory");
            new DirectoryInfo(publishDir.MakeAbsolute(Environment).FullPath).Copy(destDirPath: targetPath,
                copySubDirs: true);
            return output;
        }

        #region Cake support properties

        private MSBuildSettings BuildSettings { get; set; }
        private IProcessRunner Runner { get; set; }

        private ICakeEnvironment Environment { get; set; }
        private IFileSystem FileSystem { get; set; }
        private IToolLocator ToolLocator { get; set; }

        #endregion

        private Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        private MSBuildPlatform GetMSBuildPlatform(string platform)
        {
            return platform == "AnyCPU"
                ? MSBuildPlatform.Automatic
                : platform == "x64"
                    ? MSBuildPlatform.x64
                    : MSBuildPlatform.x86;
        }

        private void PrepareManifestManager(DirectoryPath targetPath, InformationSource infoSource)
        {
            if (!string.IsNullOrWhiteSpace(targetPath.FullPath) && GenerateManifest)
            {
                ManifestManager = new ManifestManager(ProjectFilePath, targetPath.FullPath, infoSource);
            }
        }


        private void Log(string content, bool isBuildMessage = false)
        {
            foreach (var logger in Loggers.Where(l => isBuildMessage ? l.IncludeBuildMessages : l != null))
            {
                logger.Log($"{DateTime.Now:T} - {content}");
            }
        }

        private void CloseLoggers(string targetPath)
        {
            var path = Path.Combine(targetPath, "Logs");
            var directory = FileSystem.GetDirectory(Path.Combine(targetPath, "Logs"));
            directory.Create();
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

        private Action<CakePublishManager> DefaultBuildAction => mgr =>
        {
            var runner = new MSBuildRunner(mgr.FileSystem, mgr.Environment, mgr.Runner, mgr.ToolLocator);
            runner.Run(mgr.ProjectFilePath, mgr.BuildSettings);
        };
    }
}