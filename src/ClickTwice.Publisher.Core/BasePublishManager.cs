using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace ClickTwice.Publisher.Core
{
    public abstract class BasePublishManager : Manager, IPublishManager
    {
        public BasePublishManager(string projectFilePath) : base(projectFilePath)
        {
            
        }
        public virtual void Dispose()
        {
            //do nothing
        }
        public virtual string Configuration { protected get; set; }
        public virtual string Platform { protected get; set; }
        public List<IInputHandler> InputHandlers { protected get; set; } = new List<IInputHandler>();
        public List<IOutputHandler> OutputHandlers { protected get; set; } = new List<IOutputHandler>();
        public List<IBuildConfigurator> BuildConfigurators { protected get; set; } = new List<IBuildConfigurator>();
        public List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();

        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        /// <exception cref="HandlerProcessingException">Thrown when input or output handlers encounter an exception.</exception>
        /// <exception cref="OperationInProgressException">Thrown when a build or publish operation is already in progress.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid behaviour type provided.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="BuildFailedException">Thrown when the build fails.</exception>
        public virtual List<HandlerResponse> PublishApp(string targetPath,
            PublishBehaviour behaviour = PublishBehaviour.CleanFirst)
        {
            var path = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "ClickTwice", Guid.NewGuid().ToString("N") + "\\"));
            if (!File.Exists(ProjectFilePath))
            {
                throw new FileNotFoundException(
                    $"Project file not found at path {ProjectFilePath}! Please ensure you have provided a valid csproj file.",
                    ProjectFilePath);
            }
            Log("Configuring build environment");
            var props = new Dictionary<string, string>
            {
                {"Configuration", Configuration},
                {"Platform", Platform},
                {"OutputPath", path.FullName}
            };

            props = props.Concat(AdditionalProperties.Where(p => !props.ContainsKey(p.Key)))
                .ToDictionary(k => k.Key, v => v.Value);
            Log("Processing input handlers");
            var results = ProcessInputHandlers();
            Log($"Completed processing input handlers: {results.Count(r => r.Result == HandlerResult.OK)} OK, {results.Count(r => r.Result == HandlerResult.Error)} errors, {results.Count(r => r.Result == HandlerResult.NotRun)} not run");
            if (results.All(r => r.Result != HandlerResult.Error))
            {
                var targets = behaviour.ToTargets();
                Log("Running additional configurators");
                var configurators = RunConfigurators(props, targets);
                props = configurators.Key;
                targets = configurators.Value;
                Log($"Build configured for targets: {string.Join(", ", targets)}");
                List<HandlerResponse> outResults = new List<HandlerResponse>();
                var success = BuildProject(props, targets);
                if (success)
                {
                    var publishDir = path.GetDirectories().FirstOrDefault(d => d.Name == "app.publish");
                    PostBuild(publishDir);
                    Log("Processing output handlers");
                    outResults = ProcessOutputHandlers(publishDir);
                    Log(
                        $"Completed processing output handlers: {outResults.Count(r => r.Result == HandlerResult.OK)} OK, {outResults.Count(r => r.Result == HandlerResult.Error)} errors, {outResults.Count(r => r.Result == HandlerResult.NotRun)} not run");
                    if (outResults.Any(o => o.Result == HandlerResult.Error))
                    {
                        Log("Error encountered while processing output handlers. Aborting!");
                        throw new HandlerProcessingException(OutputHandlers, outResults);
                    }
                    if (!string.IsNullOrWhiteSpace(targetPath))
                    {
                        Log("Copying publish results to target directory");
                        publishDir.Copy(destDirPath: targetPath, copySubDirs: true);
                    }
                }
                else
                {
                    throw new BuildFailedException();
                }
                CloseLoggers(targetPath);
                if (!CleanOutputOnCompletion) return outResults;
                Log("Cleaning build output directory");
                Directory.Delete(path.FullName, true);
                return outResults;
            }
            Log("Error encountered while processing input handlers. Aborting!");
            throw new HandlerProcessingException(InputHandlers, results);
        }

        protected abstract bool BuildProject(Dictionary<string, string> props, List<string> targets);

        protected virtual void PostBuild(FileSystemInfo targetPath)
        {
            
        }

        protected virtual void OnBuildFailed(Exception ex)
        {
            
        }

        protected void Log(string content, bool isBuildMessage = false)
        {
            foreach (var logger in Loggers.Where(l => isBuildMessage ? l.IncludeBuildMessages : l != null))
            {
                logger.Log($"{DateTime.Now.ToString("T")} - {content}");
            }
        }

        /// <summary>
        /// Reads the assembly version number from AssemblyInfo.cs and updates it in the project file
        /// </summary>
        protected void SyncVersionNumber()
        {

            var v = ReadVersionFromAssemblyInfo();
            SaveVersionToProjectFile(v);
        }

        protected void SaveVersionToProjectFile(string v)
        {
            var doc = XDocument.Load(ProjectFilePath);
            var vElement = doc.XPathSelectElement(
                "//*[local-name()='Project']/*[local-name()='Property-Group']/*[local-name()='ApplicationVersion']");
            vElement.Value = v;
            doc.Save(ProjectFilePath);
        }

        protected List<HandlerResponse> ProcessInputHandlers()
        {
            /* var fi = new FileInfo(ProjectFilePath).Directory?.FullName;
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
            return list; */
            return InputHandlers.ProcessHandlers(new FileInfo(ProjectFilePath).Directory?.FullName, s => Log(s));
        }

        protected List<HandlerResponse> ProcessOutputHandlers(FileSystemInfo outputPath)
        {
            var results =
                OutputHandlers.Where(_ => !string.IsNullOrWhiteSpace(outputPath?.FullName))
                    .Select(handler => handler.Process(outputPath.FullName))
                    .ToList();
            return results;
        }

        protected KeyValuePair<Dictionary<string, string>, List<string>> RunConfigurators(Dictionary<string, string> props, List<string> targetList)
        {
            var properties = props;
            var targets = targetList;
            foreach (var configurator in BuildConfigurators)
            {
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

        protected void CloseLoggers(string targetPath)
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
    }
}
