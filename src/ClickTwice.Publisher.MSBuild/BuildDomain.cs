using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Loggers;
using ClickTwice.Publisher.MSBuild.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ClickTwice.Publisher.MSBuild
{
    /// This class is not currently in use and is preserved only for experimental use in future releases
    [Serializable]
    class BuildDomain : MarshalByRefObject
    {

        private static BuildManager Manager => BuildManager.DefaultBuildManager;

        internal static Action<string> Log { get; set; }

        internal static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',').FirstOrDefault();
            Log($"Resolving {assemblyName}");
            return assemblyName == null ? Assembly.Load(args.Name) : Assembly.Load(assemblyName);
        }

        internal static void Dom_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Log($" Loading {args.LoadedAssembly.FullName}");
        }

        internal static bool Build(string projectFilePath, Dictionary<string, string> props, List<string> targets, IEnumerable<IPublishLogger> publishLoggers)
        {
            var pc = new ProjectCollection();
            Log("Configuring loggers");
            var loggers = new List<ILogger> { new BuildMessageLogger(publishLoggers.Where(l => l.IncludeBuildMessages).ToList()) };
            Log($"Added {loggers.Count} loggers");
            var buildParams = new BuildParameters(pc)
            {
                DetailedSummary = true,
                Loggers = loggers,
                DefaultToolsVersion = "14.0"
            };
            var reqData = new BuildRequestData(projectFilePath, props, "14.0", targets.ToArray(), null);
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
            if (buildResult.OverallResult == BuildResultCode.Success)
            {
                return true;
            }
            if (buildResult.Exception?.Message != null)
            {
                Log($"MSBuild build failed with {buildResult.Exception.GetType().Name}: {buildResult.Exception.Message}");
            }
            return false;
        }
    }
}
