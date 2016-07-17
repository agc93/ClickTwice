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
    class CakePublishManager : Manager, IPublishManager
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
                AdditionalProperties.Add("ApplicationVersion", mgr.PublishVersion);
        }

        private Action<IEnumerable<HandlerResponse>> ErrorAction { get; set; }

        private Action<CakePublishManager> BuildAction { get; set; }

        public void Dispose()
        {
            
        }

        public string Configuration { private get; set; }
        public string Platform { private get; set; }
        private bool GenerateManifest { get; set; } = true;
        public List<IInputHandler> InputHandlers { private get; set; }
        public List<IOutputHandler> OutputHandlers { get; set; }
        public List<IBuildConfigurator> BuildConfigurators { get; set; }
        public List<IPublishLogger> Loggers { get; } = new List<IPublishLogger>();
        private ManifestManager ManifestManager { get; set; }

        public List<HandlerResponse> PublishApp(string targetPath, PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var results = InputHandlers.ProcessHandlers(
                new FilePath(ProjectFilePath).GetDirectory().MakeAbsolute(Environment).FullPath, s => Log(s));
            Log($"Completed processing input handlers: {results.Count(r => r.Result == HandlerResult.OK)} OK, {results.Count(r => r.Result == HandlerResult.Error)} errors, {results.Count(r => r.Result == HandlerResult.NotRun)} not run");
            if (results.Any(r => r.Result == HandlerResult.Error)) throw new HandlerProcessingException(InputHandlers, results);
            string outputPath;
            if (behaviour == PublishBehaviour.DoNotBuild)
            {
                outputPath = Path.Combine(new FileInfo(ProjectFilePath).Directory.FullName, "bin", Configuration);
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
            BuildAction?.Invoke(this);
            var publishDir =
                new DirectoryPath(new DirectoryInfo(props["OutputPath"]).GetDirectories().FirstOrDefault(d => d.Name == "app.publish").FullName);
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
            return platform == "AnyCPU" ? MSBuildPlatform.Automatic : platform == "x64" ? MSBuildPlatform.x64 : MSBuildPlatform.x86;
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
                logger.Log($"{DateTime.Now.ToString("T")} - {content}");
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

    internal static class CakePublishExtensions
    {
        internal static void AddTargets(this MSBuildSettings settings, PublishBehaviour behaviour)
        {
            foreach (var target in GetTargets(behaviour))
            {
                settings.WithTarget(target);
            }
        }
        private static List<string> GetTargets(PublishBehaviour behaviour)
        {
            var targets = new List<string> { "PrepareForBuild" };
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
            return targets;
        }

        internal static void AddProperties(this MSBuildSettings settings, params Dictionary<string, string>[] dicts)
        {
            var props = dicts.First();
            foreach (var dict in dicts.Skip(1))
            {
                props = props.Concat(dict.Where(p => !props.ContainsKey(p.Key)))
                .ToDictionary(k => k.Key, v => v.Value);
            }
            foreach (var prop in props)
            {
                settings.WithProperty(prop.Key, prop.Value);
            }
        }
    }
}

