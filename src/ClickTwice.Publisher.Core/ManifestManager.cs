using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core
{
    class ManifestManager : Manager
    {
        public ManifestManager(string projectFilePath, InformationSource source = InformationSource.AppManifest) : base(projectFilePath)
        {
            Source = source;
        }

        public ManifestManager(string projectFilePath, string applicationManifestFilePath) : base(projectFilePath)
        {
            Source = InformationSource.AppManifest;
            ApplicationManifestLocation = applicationManifestFilePath;
        }

        private string ApplicationManifestLocation { get; set; }

        private InformationSource Source { get; set; }

        public AppManifest CreateAppManifest()
        {
            AppManifest manifest;
            switch (Source)
            {
                case InformationSource.AssemblyInfo:
                    manifest = CreateFromAssemblyInfo();
                    break;
                case InformationSource.AppManifest:
                    manifest = CreateFromDeployManifest();
                    break;
                case InformationSource.None:
                    manifest = new AppManifest();
                    break;
                case InformationSource.Both:
                    manifest = CreateFromAssemblyInfo();
                    manifest = CreateFromDeployManifest(manifest);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return manifest;
        }

        private AppManifest CreateFromDeployManifest(AppManifest manifest)
        {
            var xdoc = XDocument.Load(ApplicationManifestLocation);
            var descEl = xdoc.XPathSelectElement("//*[local-name()='description']");
            manifest.ApplicationName = descEl.FindAttribute("product");
            manifest.PublisherName = descEl.FindAttribute("publisher");
            manifest.SuiteName = descEl.FindAttribute("suiteName");
            var identEl = xdoc.XPathSelectElement("//*[local-name()='assemblyIdentity']");
            manifest.AppVersion = new Version(identEl.FindAttribute("version"));
            manifest.ShortName = identEl.FindAttribute("name").Split('.').First();
            var frameworksRoot = xdoc.XPathSelectElement("//*[local-name()='compatibleFrameworks']");
            manifest.FrameworkVersion = new Version(frameworksRoot.Elements("framework").OrderByDescending(x => x.FindAttribute("targetVersion")).First().FindAttribute("targetVersion"));
            return manifest;
        }

        private AppManifest CreateFromDeployManifest()
        {
            return CreateFromDeployManifest(new AppManifest());
        }

        private AppManifest CreateFromAssemblyInfo(AppManifest manifest)
        {
            var projectFolder = new FileInfo(ProjectFilePath).Directory;
            var infoFilePath = Path.Combine(projectFolder.FullName, "Properties", "AssemblyInfo.cs");
            if (File.Exists(infoFilePath))
            {
                var props = File.ReadAllLines(infoFilePath).Where(l => l.StartsWith("[assembly: ")).ToList();
                manifest.ApplicationName = props.Property("AssemblyTitle");
                manifest.Description = props.Property("AssemblyDescription");
                manifest.PublisherName = props.Property("AssemblyCompany");
                manifest.SuiteName = props.Property("AssemblyProduct");
                manifest.Copyright = props.Property("Copyright");
                manifest.AppVersion = new Version(props.Property("Version"));
                return manifest;
            }
            return null;
        }

        private AppManifest CreateFromAssemblyInfo()
        {
            return CreateFromAssemblyInfo(new AppManifest());
        }

        public void CreateWebManifest()
        {
        }
    }

    public enum InformationSource
    {
        AssemblyInfo,
        AppManifest,
        Both,
        None
    }
}
