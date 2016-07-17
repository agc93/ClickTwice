using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ClickTwice.Publisher.Core.Manifests;
using Newtonsoft.Json;

namespace ClickTwice.Publisher.Core
{
    public class ManifestManager : Manager
    {
        private ManifestManager(string projectFilePath, InformationSource source = InformationSource.AssemblyInfo) : base(projectFilePath)
        {
            Source = source;
        }

        public ManifestManager(string projectFilePath, string applicationManifestFilePath, InformationSource source = InformationSource.AppManifest) : base(projectFilePath)
        {
            Source = source;
            if (applicationManifestFilePath.EndsWith(".application"))
            {
                ApplicationManifestLocation = applicationManifestFilePath;
            }
            else
            {
                var di = new DirectoryInfo(applicationManifestFilePath);
                ApplicationManifestLocation = di.GetFiles("*.application").First().FullName;
            }
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

        private AppManifest CreateFromDeployManifest(AppManifest manifest = null)
        {
            manifest = manifest ?? new AppManifest();
            var xdoc = XDocument.Load(ApplicationManifestLocation);
            var descEl = xdoc.XPathSelectElement("//*[local-name()='description']");
            manifest.ApplicationName = descEl.FindAttribute("product");
            manifest.PublisherName = descEl.FindAttribute("publisher");
            manifest.SuiteName = descEl.FindAttribute("suiteName");
            var identEl = xdoc.XPathSelectElement("//*[local-name()='assemblyIdentity']");
            manifest.AppVersion = new Version(identEl.FindAttribute("version"));
            manifest.ShortName = identEl.FindAttribute("name").Split('.').First();
            var frameworksRoot = xdoc.XPathSelectElement("//*[local-name()='compatibleFrameworks']");
            manifest.FrameworkVersion = new Version(frameworksRoot.Elements().OrderByDescending(x => x.FindAttribute("targetVersion")).First().FindAttribute("targetVersion"));
            return manifest;
        }

        private AppManifest CreateFromAssemblyInfo(AppManifest manifest = null)
        {
            manifest = manifest ?? new AppManifest();
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

        public FileInfo DeployManifest(AppManifest manifest)
        {
            var j = JsonConvert.SerializeObject(manifest, Formatting.Indented, new Newtonsoft.Json.Converters.VersionConverter());
            File.WriteAllText(GetPublishLocation(), j);
            return new FileInfo(GetPublishLocation());
        }

        private string GetPublishLocation()
        {
            var fi = new FileInfo(ApplicationManifestLocation);
            return Path.Combine(fi.Directory?.FullName ?? new FileInfo(ProjectFilePath).Directory?.FullName ?? string.Empty, fi.Name.Replace(fi.Extension, ".cltw"));
        }

        public static AppManifest ReadFromFile(string manifestFilePath)
        {
            return JsonConvert.DeserializeObject<AppManifest>(File.ReadAllText(manifestFilePath));
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
