using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;
using Newtonsoft.Json;

namespace ClickTwice.Publisher.Core.Resources
{
    partial class PublishPage
    {

        internal AppManifest Manifest { get; }
        internal FileInfo AppLauncher { get; set; }

        internal PublishPage(string deploymentPath)
        {
            this.DeploymentDir = new DirectoryInfo(deploymentPath);
            var infos = this.DeploymentDir.GetFiles("*.cltw");
            var mgr = new ManifestManager(string.Empty, deploymentPath, InformationSource.AppManifest);
            var manifest = mgr.CreateAppManifest();
            this.Manifest = manifest;
            AppLauncher = DeploymentDir.GetFiles("*.application").First();
            if (Manifest.FrameworkVersion == null)
            {
                Manifest.FrameworkVersion = new Version(4,5);
            }
        }

        internal DirectoryInfo DeploymentDir { get; }

        internal string Generate()
        {
            return this.TransformText();
        }

        internal void CreatePage()
        {
            var page = Generate();
            File.WriteAllText(Path.Combine(DeploymentDir.FullName, "publish.htm"), page);
        }

        internal ExtendedAppInfo AppInfo { get; private set; }
    }
}
