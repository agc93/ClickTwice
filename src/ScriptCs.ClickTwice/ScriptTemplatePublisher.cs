using System;
using System.Linq;
using ClickTwice.Templating;

namespace ScriptCs.ClickTwice
{
    public class ScriptTemplatePublisher : ITemplatePublisher
    {
        public ScriptTemplatePublisher(string templateDirectory)
        {
            TemplateDirectory = templateDirectory;
        }

        private string TemplateDirectory { get; }

        public ScriptTemplatePublisher SetMetadata(string id, string version, string author, string description = null)
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

        public TemplatePackageSettings Metadata { get; private set; }

        public ScriptTemplatePublisher SetMetadata(TemplatePackageSettings settings)
        {
            Metadata = settings;
            return this;
        }

        public ScriptTemplatePublisher EnableVisualStudioMode()
        {
            PackagingMode = PackagingMode.VisualStudio;
            return this;
        }

        public PackagingMode PackagingMode { get; set; } = PackagingMode.Minimal;

        public ITemplatePublisher ToPackageFile(string outputPath)
        {
            var mgr = new TemplatePackager(Metadata);
            var fi = mgr.Package(TemplateDirectory, PackagingMode);
            fi.CopyTo(outputPath);
            return this;
        }

        public ITemplatePublisher ToGallery(string apiKey = null, string galleryUri = null)
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