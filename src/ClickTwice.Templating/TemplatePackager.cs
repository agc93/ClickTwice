using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ClickTwice.Publisher.Core;
using NuGet;

namespace ClickTwice.Templating
{
    public class TemplatePackager
    {
        public Uri PublishDestination { get; set; } = new Uri("http://nudev.azurewebsites.net");
        private Manifest NuSpec { get; set; }
        public TemplatePackager(string id, string version, string authors, string description)
        {
            var manifest = new Manifest
            {
                Metadata =
                {
                    Id = id,
                    Version = version,
                    Authors = authors,
                    Description = description
                }
            };
            NuSpec = manifest;
        }

        public TemplatePackager(TemplatePackageSettings settings) : this(settings.Id, settings.Version, string.Join(", ", settings.Authors), settings.Description)
        {
        }

        private void SetContentFiles(List<string> contentFiles)
        {
            NuSpec.Files = new List<ManifestFile>();
            contentFiles = contentFiles.Select(f => f.Contains("\\") ? f.Replace("\\", "/") : f).ToList();
            NuSpec.Files.AddRange(contentFiles.Select(f => new ManifestFile() {Source = f, Target = $"content/{f}"}));
        }
        private IPackager Packager { get; set; }
        public FileInfo Package(string templateProjectDirectory, PackagingMode mode)
        {
            switch (mode)
            {
                case PackagingMode.VisualStudio:
                    Packager = new VisualStudioPackager();
                    break;
                case PackagingMode.Minimal:
                    Packager = new MinimalPackager();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            var files = Packager.GetContentFiles(templateProjectDirectory);
            SetContentFiles(files);
            using (var stream = new FileStream(Path.Combine(templateProjectDirectory, "TemplatePackage.nuspec"), FileMode.Create))
            {
                //NuSpec.Save(stream, validate: true);
            }
            //NuSpec.Save(new FileStream(Path.Combine(templateProjectDirectory, "TemplatePackage.nuspec"), FileMode.Create), validate: true);
            var package = BuildPackage(templateProjectDirectory);
            return package;
        }

        private FileInfo BuildPackage(string outputDirectory)
        {
            var builder = new PackageBuilder();
            builder.Populate(NuSpec.Metadata);
            builder.PopulateFiles(outputDirectory, NuSpec.Files);
            var path = Path.Combine(outputDirectory, $"{NuSpec.Metadata.Id}.nupkg");
            using (
                FileStream stream = new FileStream(path, FileMode.Create))
            {
                builder.Save(stream);
            }
            return new FileInfo(path);
        }

        public void PublishPackage(string pathToNupkg)
        {
            PublishPackage(pathToNupkg, System.Environment.GetEnvironmentVariable("ApiKey"));
        }

        public void PublishPackage(string pathToNupkg, string apiKey)
        {
            var fi = new FileInfo(pathToNupkg);
            var localRepo = PackageRepositoryFactory.Default.CreateRepository(fi.Directory.FullName);
            var package = localRepo.FindPackagesById(fi.Name.Replace(fi.Extension, string.Empty)).First();
            var size = fi.Length;
            var ps = new PackageServer(PublishDestination.ToString(), "userAgent");
            ps.PushPackage(apiKey, package, size, 1800, false);
        }
    }

    internal interface IPackager
    {
        List<string> GetContentFiles(string rootDirectory);
    }

    internal class VisualStudioPackager : IPackager
    {
        public List<string> GetContentFiles(string rootDirectory)
        {
            var xml = new XmlDocument();
            xml.Load(new DirectoryInfo(rootDirectory).GetFiles("*.csproj").First().FullName);
            XmlNamespaceManager mgr = new XmlNamespaceManager(xml.NameTable);
            mgr.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
            var nodeList = xml.SelectNodes("//msb:Project/msb:ItemGroup/msb:Content", mgr);
            if (nodeList != null)
            {
                var contentFiles =
                    nodeList.Cast<XmlNode>()
                        .Where(x => !string.IsNullOrWhiteSpace(x.Attributes?["Include"].Value))
                        .Select(x => x.Attributes["Include"].Value);
                return contentFiles.Where(f => !f.EndsWith("config")).ToList();
            }
            return new List<string>();
        }
    }

    internal class MinimalPackager : IPackager
    {
        public List<string> GetContentFiles(string rootDirectory)
        {
            var files = new DirectoryInfo(rootDirectory).EnumerateFilesForExtensions(false, ".nupkg", ".nuspec", ".config");
            return
                files.Select(f => f.FullName)
                    .Select(n => n.Replace(rootDirectory, string.Empty).Trim().TrimStart('\\'))
                    .ToList();
            //return
            //    new DirectoryInfo(rootDirectory).GetFilesExceptExtensions(".nupkg", ".nuspec", ".dll", ".config")
            //        .Select(f => f.FullName.Replace(rootDirectory, string.Empty))
            //        .ToList();
        }
    }

    public enum PackagingMode
    {
        VisualStudio,
        Minimal
    }
}
