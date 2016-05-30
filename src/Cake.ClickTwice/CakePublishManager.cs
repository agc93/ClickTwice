using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using Path = System.IO.Path;

namespace Cake.ClickTwice
{
    class CakePublishManager : Manager, IPublishManager
    {
        public CakePublishManager(ICakeEnvironment env, IFileSystem fs, IProcessRunner runner, IGlobber globber, string projectFilePath, InformationSource source = InformationSource.AssemblyInfo, Action<CakePublishManager> buildAction = null) : base(projectFilePath)
        {
            Environment = env;
            FileSystem = fs;
            Runner = runner;
            Globber = globber;
            BuildAction = buildAction ?? (mgr =>
            {
                var builder = new MSBuildRunner(FileSystem, Environment, Runner, Globber);
                builder.Run(ProjectFilePath, BuildSettings);
            });
        }

        internal Action<CakePublishManager> BuildAction { get; set; }

        public void Dispose()
        {
            
        }

        public string Configuration { private get; set; } = "Debug";
        public string Platform { private get; set; } = "AnyCPU";
        private bool GenerateManifest { get; set; } = true;
        public List<IInputHandler> InputHandlers { get; set; } = new List<IInputHandler>();
        public List<IOutputHandler> OutputHandlers { get; set; } = new List<IOutputHandler>();
        public List<IBuildConfigurator> BuildConfigurators { get; set; }
        public List<IPublishLogger> Loggers { get; } = new List<IPublishLogger>();
        private ManifestManager ManifestManager { get; set; }
        private MSBuildSettings BuildSettings { get; set; }
        public List<HandlerResponse> PublishApp(string targetPath, PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var results =
                ProcessInputHandlers(new FilePath(ProjectFilePath).GetDirectory().MakeAbsolute(Environment).FullPath);
            Log($"Completed processing input handlers: {results.Count(r => r.Result == HandlerResult.OK)} OK, {results.Count(r => r.Result == HandlerResult.Error)} errors, {results.Count(r => r.Result == HandlerResult.NotRun)} not run");
            if (results.Any(r => r.Result == HandlerResult.Error)) throw new HandlerProcessingException(InputHandlers, results);
            var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + "\\");
            var props = new Dictionary<string, string>
            {
                {"Configuration", Configuration},
                {"Platform", Platform},
                {"OutputPath", outputPath},
                {"PublishUrl", Path.Combine(outputPath, "app.publish") }
            };
            BuildSettings = new MSBuildSettings
            {
                Configuration = Configuration,
                MSBuildPlatform = GetMSBuildPlatform(Platform),
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
            var output = ProcessOutputHandlers(publishDir);
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
                new DirectoryInfo(publishDir.MakeAbsolute(Environment).FullPath).Copy(destDirPath: targetPath,
                    copySubDirs: true);
            }
            return output;
        }

        private IGlobber Globber { get; set; }

        private IProcessRunner Runner { get; set; }

        private ICakeEnvironment Environment { get; set; }
        private IFileSystem FileSystem { get; set; }


        internal Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        private MSBuildPlatform GetMSBuildPlatform(string platform)
        {
            return platform == "AnyCPU" ? MSBuildPlatform.Automatic : platform == "x64" ? MSBuildPlatform.x64 : MSBuildPlatform.x86;
        }

        private List<HandlerResponse> ProcessInputHandlers(string inputDirectoryPath)
        {
            var list = new List<HandlerResponse>();
            if (string.IsNullOrWhiteSpace(inputDirectoryPath))
            {
                return null;
            }
            foreach (var handler in InputHandlers)
            {
                try
                {
                    var result = handler.Process(inputDirectoryPath);
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

        private void PrepareManifestManager(DirectoryPath targetPath, InformationSource infoSource)
        {
            if (!string.IsNullOrWhiteSpace(targetPath.FullPath) && GenerateManifest)
            {
                ManifestManager = new ManifestManager(ProjectFilePath, targetPath.FullPath, infoSource);
            }
        }

        private List<HandlerResponse> ProcessOutputHandlers(DirectoryPath outputPath)
        {
            var results =
                OutputHandlers.Where(_ => !string.IsNullOrWhiteSpace(outputPath?.FullPath))
                    .Select(handler => handler.Process(outputPath.FullPath))
                    .ToList();
            return results;
        }
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
