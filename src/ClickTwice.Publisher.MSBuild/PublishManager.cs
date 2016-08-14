using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using ClickTwice.Publisher.MSBuild.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ClickTwice.Publisher.MSBuild
{
    public class PublishManager : BasePublishManager
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

        public override string Configuration { protected get; set; } = "Release";
        public override string Platform { protected get; set; } = "x86";

        

        protected override bool BuildProject(Dictionary<string, string> props, List<string> targets)
        {
            return Build(props, targets);

        }

        private bool Build(Dictionary<string, string> props, List<string> targets)
        {
            var pc = new ProjectCollection();
            Log("Configuring loggers");
            var loggers = new List<ILogger> { new BuildMessageLogger(Loggers.Where(l => l.IncludeBuildMessages).ToList()) };
            Log($"Added {loggers.Count} loggers");
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



        protected override void PostBuild(FileSystemInfo targetPath)
        {
            base.PostBuild(targetPath);
            if (!GenerateManifest) return;
            Log("Generating ClickTwice manifest");
            PrepareManifestManager(targetPath?.FullName);
            ManifestManager.DeployManifest(ManifestManager.CreateAppManifest());
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',').FirstOrDefault();
            Log($"Resolving {assemblyName}");
            return assemblyName == null ? Assembly.Load(args.Name) : Assembly.Load(assemblyName);
        }


        private void PrepareManifestManager(string targetPath)
        {
            if (!string.IsNullOrWhiteSpace(targetPath) && GenerateManifest)
            {
                ManifestManager = new ManifestManager(ProjectFilePath, targetPath, ManifestInformationSource);
            }
        }
        
        public override void Dispose()
        {
            /* Manager.EndBuild();
            Manager.Dispose(); */
        }
    }
}