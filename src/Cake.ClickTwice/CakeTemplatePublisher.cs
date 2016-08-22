using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Templating;

namespace Cake.ClickTwice
{
    public class CakeTemplatePublisher : ITemplatePublisher
    {
        public CakeTemplatePublisher(string templateDirectory, TemplatePackageSettings settings)
        {
            Metadata = settings;
            Packager = new TemplatePackager(settings);
            TemplateDirectory = templateDirectory;
        }

        private string TemplateDirectory { get; set; }

        private TemplatePackager Packager { get; set; }

        public TemplatePackageSettings Metadata { get; }
        public PackagingMode PackagingMode { get; } = PackagingMode.Minimal;
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
