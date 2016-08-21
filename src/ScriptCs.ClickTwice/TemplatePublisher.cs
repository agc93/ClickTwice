using System;
using System.Linq;
using ClickTwice.Templating;

namespace ScriptCs.ClickTwice
{
    public class TemplatePublisher
    {
        public TemplatePublisher(string templateDirectory)
        {
            TemplateDirectory = templateDirectory;
        }

        private string TemplateDirectory { get; set; }

        public TemplatePublisher SetMetadata(string id, string version, string author, string description)
        {
            Metadata = new TemplatePackageSettings()
            {
                Id = id,
                Version = version,
                Description = description,
                Authors = author.Split(',', ';').ToList()
            };
            return this;
        }

        private TemplatePackageSettings Metadata { get; set; }

        public TemplatePublisher SetMetadata(TemplatePackageSettings settings)
        {
            Metadata = settings;
            return this;
        }

        public TemplatePublisher EnableVisualStudioMode()
        {
            PackagingMode = PackagingMode.VisualStudio;
            return this;
        }

        private PackagingMode PackagingMode { get; set; } = PackagingMode.Minimal;

        public TemplatePublisher ToPackageFile(string outputPath)
        {
            var mgr = new TemplatePackager(Metadata);
            var fi = mgr.Package(TemplateDirectory, PackagingMode);
            fi.CopyTo(outputPath);
            return this;
        }

        public TemplatePublisher ToGallery(string apiKey = null, string galleryUri = null)
        {
            var mgr = new TemplatePackager(Metadata)
            {
                PublishDestination = new Uri(galleryUri ?? "https://nuget.org/api/v2")
            };
            if (apiKey == null)
            {
                mgr.PublishPackage(mgr.Package(TemplateDirectory, PackagingMode).FullName);
            }
            else
            {
                mgr.PublishPackage(mgr.Package(TemplateDirectory, PackagingMode).FullName, apiKey);
            }
            return this;
        }
    }
}