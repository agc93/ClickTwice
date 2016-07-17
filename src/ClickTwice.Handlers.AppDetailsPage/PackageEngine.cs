using System.IO;
using NuGet;

namespace ClickTwice.Handlers.AppDetailsPage
{
    class PackageEngine
    {
        private DefaultPackagePathResolver Resolver { get; set; }

        private PackageEngine()
        {
            Resolver = new DefaultPackagePathResolver(Path.Combine(Path.GetTempPath(), "TemplatePackages"));
            FileSystem = new PhysicalFileSystem(Path.Combine(Path.GetTempPath(), "Templates"));
        }
        internal PackageEngine(string packageId, string repoSource) : this()
        {
            Repository = PackageRepositoryFactory.Default.CreateRepository(repoSource);
            Manager = new PackageManager(Repository, Resolver, FileSystem);
            Package = Repository.FindPackage(packageId, new VersionSpec() {MinVersion = new SemanticVersion(0, 0, 0, 0)},
                true, true);
        }

        internal PackageEngine(FileInfo localPackage) : this()
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "LocalTemplates"));
            localPackage.CopyTo(Path.Combine(dir.FullName, localPackage.Name), true);
            Repository = new LocalPackageRepository(dir.FullName, true);
            Manager = new PackageManager(Repository, Resolver, FileSystem);
            Package = Repository.FindPackage(localPackage.Name.Replace(localPackage.Extension, string.Empty), new VersionSpec() {MinVersion = new SemanticVersion(0, 0, 0, 0)},
                true, true);
        }

        internal DirectoryInfo ExtractPackage()
        {
            var dirPath = Path.Combine(Path.GetTempPath(), "Templates", Package.Id);
            var packageFiles = Package.GetContentFiles();
            Package.ExtractContents(FileSystem, dirPath);
            return new DirectoryInfo(Path.Combine(dirPath, "content"));
        }

        private IPackage Package { get; set; }

        private PackageManager Manager { get; set; }

        private PhysicalFileSystem FileSystem { get; set; }

        private IPackageRepository Repository { get; set; }
    }
}
