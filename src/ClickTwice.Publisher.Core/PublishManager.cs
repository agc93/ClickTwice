using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Tasks;

namespace ClickTwice.Publisher.Core
{
    public class PublishManager : IDisposable
    {
        private BuildManager Manager => BuildManager.DefaultBuildManager;

        public PublishManager(string projectFilePath)
        {
            ProjectFilePath = projectFilePath;
        }

        internal string ProjectFilePath { get; set; }
        public string Configuration { private get; set; } = "Release";
        public string Platform { private get; set; } = "x86";

        public BuildResult PublishApp()
        {
            var pi = new ProjectInstance(ProjectFilePath);
            var targets = new[] {Configuration + ";" + Platform};
            BuildParameters buildParams = new BuildParameters()
            {
                DetailedSummary = true
            };
            var reqData = new BuildRequestData(pi, targets);
            Manager.BeginBuild(buildParams);
            var buildResult = Manager.BuildRequest(reqData);
            return buildResult;
        }

        public BuildResult PublishApp(PublishBehaviour behaviour)
        {
            var pc = new ProjectCollection();
            var loggers = new List<ILogger> {new ConsoleLogger(LoggerVerbosity.Normal)};
            var path = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
            var props = new Dictionary<string, string> {{"Configuration", Configuration}, {"Platform", Platform}, {"OutputPath", path.FullName} };
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
                Loggers =  loggers,
                DefaultToolsVersion = "14.0"
            };
            var reqData = new BuildRequestData(ProjectFilePath, props, null, targets.ToArray(), null);
            try
            {
                Manager.BeginBuild(buildParams);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("in progress"))
            {
                    throw new OperationInProgressException(OperationType.Build);
            }
            var buildResult = Manager.BuildRequest(reqData);
            if (buildResult.OverallResult == BuildResultCode.Success)
            {
            }
            return buildResult;
        }

        public void Dispose()
        {
            Manager.EndBuild();
            Manager.Dispose();
        }
    }

    public class OperationInProgressException : Exception
    {
        public OperationInProgressException(OperationType operationType)
        {
            this.Operation = operationType;
        }

        public OperationType Operation { get; set; }
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
