using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ClickTwice.Publisher.Core;
using NuGet;

namespace ClickTwice.CommandLine.Packaging
{
    class TemplatePackager
    {

        public Manifest NuSpec { get; set; }
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

        public TemplatePackager(string pathToNuspec)
        {
            
        }

        internal void SetContentFiles(List<string> contentFiles)
        {
            NuSpec.Files.AddRange(contentFiles.Select(f => new ManifestFile() {Source = f, Target = f}));
            //foreach (var file in contentFiles)
            //{
            //    NuSpec.Files.Add(new ManifestFile()
            //    {
            //        Source = file,
            //        Target = file
            //    });
            //}
        }
        private IPackager Packager { get; set; }
        public void Package(string templateProjectDirectory, PackagingMode mode)
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
            NuSpec.Save(new FileStream(Path.Combine(templateProjectDirectory, "TemplatePackage.nuspec"), FileMode.Create), validate: true);
            BuildPackage(templateProjectDirectory);
        }

        private void BuildPackage(string outputDirectory)
        {
            var builder = new PackageBuilder();
            builder.Populate(NuSpec.Metadata);
            foreach (ManifestFile value in NuSpec.Files)
            {
                builder.PopulateFiles(value.Target, NuSpec.Files);
                builder.Files.Add(new PhysicalPackageFile()
                {
                    SourcePath = value.Source,
                    TargetPath = value.Target
                });
            }
            using (
                FileStream stream = new FileStream(Path.Combine(outputDirectory, $"{NuSpec.Metadata.Id}.nupkg"),
                    FileMode.Create))
            {
                builder.Save(stream);
            }
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
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
            var nodeList = xml.SelectNodes("//msb:Project/msb:ItemGroup/msb:Content", mgr);
            if (nodeList != null)
            {
                var contentFiles =
                    nodeList.Cast<XmlNode>()
                        .Where(x => !string.IsNullOrWhiteSpace(x.Attributes?["Include"].Value))
                        .Select(x => x.Attributes["Include"].Value);
                return contentFiles.ToList();
            }
            return new List<string>();
        }
    }

    internal class MinimalPackager : IPackager
    {
        public List<string> GetContentFiles(string rootDirectory)
        {
            return
                new DirectoryInfo(rootDirectory).GetFilesExceptExtensions(".nupkg", ".nuspec", ".dll", ".config")
                    .Select(f => f.FullName.Replace(rootDirectory, string.Empty))
                    .ToList();
        }
    }

    public enum PackagingMode
    {
        VisualStudio,
        Minimal
    }
}
