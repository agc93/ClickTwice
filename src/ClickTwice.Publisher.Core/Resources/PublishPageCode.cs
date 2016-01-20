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

        internal AppManifest Manifest { get; private set; }
        internal FileInfo AppLauncher { get; set; }

        internal PublishPage(string deploymentPath)
        {
            this.DeploymentDir = new DirectoryInfo(deploymentPath);
            var infos = this.DeploymentDir.GetFiles("*.cltw");
            var manifest = File.ReadAllText(infos.First().FullName);
            var appManifest = JsonConvert.DeserializeObject<AppManifest>(manifest);
            this.Manifest = appManifest;
            AppLauncher = DeploymentDir.GetFiles("*.application").First();
            if (Manifest.FrameworkVersion == null)
            {
                
            }
        }

        internal DirectoryInfo DeploymentDir { get; private set; }

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
