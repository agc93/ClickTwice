using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace ClickTwice.Handlers.AppDetailsPage
{
    class PackageEngine
    {
        private DefaultPackagePathResolver Resolver { get; set; }

        internal PackageEngine(string packageId)
        {
            Repository = PackageRepositoryFactory.Default.CreateRepository("http://nudev.azurewebsites.net");
            Resolver = new DefaultPackagePathResolver(Path.Combine(Path.GetTempPath(), "TemplatePackages"));
            FileSystem = new PhysicalFileSystem(Path.Combine(Path.GetTempPath(), "Templates"));
            Manager = new PackageManager(Repository, Resolver, FileSystem);
            Package = Repository.FindPackage(packageId, new VersionSpec() {MinVersion = new SemanticVersion(0, 0, 0, 0)},
                true, true);
        }

        internal void ExtractPackage()
        {
            var packageFiles = Package.GetContentFiles();
            Package.ExtractContents(FileSystem, Path.Combine(Path.GetTempPath(), "Templates", Package.GetFullName()));
        }

        private IPackage Package { get; set; }

        private PackageManager Manager { get; set; }

        private PhysicalFileSystem FileSystem { get; set; }

        private IPackageRepository Repository { get; set; }
    }
}
