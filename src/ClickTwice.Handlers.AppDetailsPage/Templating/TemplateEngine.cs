using System;
using System.IO;
using ClickTwice.Publisher.Core.Manifests;
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
            WorkingDirectory =
                Directory.CreateDirectory(Path.Combine(directory.Parent.Parent.FullName, $"{directory.Parent.Name}.Content"));
        }

        private DirectoryInfo WorkingDirectory { get; set; }

        internal AppManifest Manifest { get; set; } = new AppManifest();
        internal ExtendedAppInfo AppInfo { get; set; } = new ExtendedAppInfo();

        internal DirectoryInfo CreateContentDirectory()
        {
            var
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
