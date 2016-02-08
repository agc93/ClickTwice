using System;
using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core.Manifests;
using ClickTwice.Publisher.Core.Resources;
using RazorEngine.Templating;

namespace ClickTwice.Handlers.AppDetailsPage.Templating
{
    class TemplateEngine : IDisposable
    {
        private IRazorEngineService Engine { get; }

        public TemplateEngine(DirectoryInfo directory)
        {
            var config = new RazorEngine.Configuration.TemplateServiceConfiguration
            {
                TemplateManager = new PackageTemplateManager(directory)
            };
            Engine = RazorEngineService.Create(config);
            SourceDirectory = directory;
            WorkingDirectory =
                Directory.CreateDirectory(Path.Combine(directory.Parent.Parent.FullName, $"{directory.Parent.Name}.Content"));
        }

        private DirectoryInfo SourceDirectory { get; set; }

        private DirectoryInfo WorkingDirectory { get; set; }

        internal AppManifest Manifest { get; set; } = new AppManifest();
        internal ExtendedAppInfo AppInfo { get; set; } = new ExtendedAppInfo();

        internal DirectoryInfo CreateContentDirectory(DirectoryInfo outputPath)
        {
            
            var model = new LaunchPageModel(Manifest, AppInfo)
            {
                ContentDirectory = "./Web Files",
                Launcher = outputPath.GetFiles("*.application").First().Name,
                Installer = outputPath.GetFiles("setup.exe").FirstOrDefault()?.Name ?? "#"
            };
            if (model.AppInfo.Links == null) model.AppInfo.Links = new LinkList();
            var webFilesDir = Directory.CreateDirectory(Path.Combine(WorkingDirectory.FullName, "Web Files"));
            var otherFiles = SourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => !f.Extension.Contains("cshtml"));
            foreach (var otherFile in otherFiles)
            {
                var targetRelativePath = otherFile.FullName.Replace(SourceDirectory.FullName, string.Empty).Substring(1);
                var fullPath = Path.Combine(webFilesDir.FullName, targetRelativePath);
                Directory.CreateDirectory(new FileInfo(fullPath).Directory.FullName);
                otherFile.CopyTo(
                    fullPath, true);
            }

            var templateFiles = SourceDirectory.EnumerateFiles("*.cshtml", SearchOption.AllDirectories).Where(f => f.Name.ToLower() != "index.cshtml");
            foreach (var templateFile in templateFiles)
            {
                var targetRelativePath = templateFile.FullName.Replace(SourceDirectory.FullName, string.Empty).Substring(1);
                var fullPath = Path.Combine(webFilesDir.FullName, targetRelativePath);
                Directory.CreateDirectory(new FileInfo(fullPath).Directory.FullName);
                var compile = Engine.RunCompile(templateFile.Name, typeof (LaunchPageModel), model);
                File.WriteAllText(compile, fullPath);
            }
            var indexFile = SourceDirectory.GetFiles("index.cshtml").FirstOrDefault();
            if (indexFile == null) throw new FileNotFoundException("Could not location 'index.cshtml' which is a required file");
            var indexDestPath = Path.Combine(WorkingDirectory.FullName,
                indexFile.FullName.Replace(indexFile.Extension, ".html").Replace(SourceDirectory.FullName, string.Empty).Substring(1));
            
            var indexOutput = Engine.RunCompile(indexFile.Name, typeof (LaunchPageModel), model);
            File.WriteAllText(indexDestPath, indexOutput);
            return WorkingDirectory;
        }

        public void Dispose()
        {
            WorkingDirectory.Delete(recursive: true);
        }
    }
}
