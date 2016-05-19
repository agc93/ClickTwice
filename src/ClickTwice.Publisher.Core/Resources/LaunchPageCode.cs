using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core.Resources
{
    partial class LaunchPage
    {
        public LaunchPageModel Model { get; set; }
        public LaunchPage(string deploymentPath, string outputFileName)
        {
            var fi = new DirectoryInfo(deploymentPath).GetFiles("*.cltw").FirstOrDefault();
            if (fi == null)
            {
                var mgr = new ManifestManager(string.Empty, deploymentPath, InformationSource.AppManifest);
                var manifest = mgr.CreateAppManifest();
                this.Manifest = manifest;
            }
            else
            {
                Manifest = ManifestManager.ReadFromFile(fi.FullName);
            }
            this.DeploymentDir = new DirectoryInfo(deploymentPath);
            
            AppLauncher = DeploymentDir.GetFiles("*.application").First();
            if (Manifest.FrameworkVersion == null)
            {
                Manifest.FrameworkVersion = new Version(4, 5);
            }
            this.Model = new LaunchPageModel(Manifest);
            this.OutputFileName = outputFileName;
        }

        private string OutputFileName { get; set; }

        internal void CreatePage()
        {
            var page = this.TransformText();
            File.WriteAllText(Path.Combine(DeploymentDir.FullName, OutputFileName), page);
        }

        public FileInfo AppLauncher { get; set; }

        public AppManifest Manifest { get; set; }

        public DirectoryInfo DeploymentDir { get; set; }
    }
}
