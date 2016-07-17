using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClickTwice.Handlers.AppDetailsPage.Templating;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Handlers.AppDetailsPage
{
    public class AppDetailsPageHandler : IOutputHandler
    {
        
        public AppDetailsPageHandler(string templateName, string source = "http://nuget.org/api/v2") : this(new PackageEngine(templateName, source))
        {
        }

        public AppDetailsPageHandler(FileInfo localPackage) : this(new PackageEngine(localPackage))
        {
        }

        private AppDetailsPageHandler(PackageEngine engine) {
            var extractPackage = engine.ExtractPackage();
            TemplateDirectory = extractPackage;
            Engine = new TemplateEngine(TemplateDirectory);
        }

        private TemplateEngine Engine { get; set; }

        private DirectoryInfo TemplateDirectory { get; set; }

        public string Name => "App Details Page generator";
        public HandlerResponse Process(string outputPath)
        {
            var di = new DirectoryInfo(outputPath);
            if (!di.Exists) throw new DirectoryNotFoundException("Invalid output path!");
            var files = di.GetFiles();
            var manifestPresent = files.Any(f => f.Extension == ".cltw");
            var infoPresent = files.Any(f => f.Name == "app.info");
            if (!(manifestPresent && infoPresent))
            {
                return new HandlerResponse(this, false, "This handler requires both a deployment manifest (cltw file) and an info file (app.info)! Try adding AppInfoHandler to your OutputHandlers and ensure you have set an InformationSource to generate a manifest.");
            }
            Manifest = ManifestManager.ReadFromFile(GetManifest(outputPath).FullName);
            AppInfo = AppInfoManager.ReadFromFile(GetInfoFile(outputPath).FullName);
            Engine.Manifest = Manifest;
            Engine.AppInfo = AppInfo;
            var contentDirectory = Engine.CreateContentDirectory(new DirectoryInfo(outputPath));
            contentDirectory.Copy(outputPath, copySubDirs: true);
            if (FileNameMap.Any())
            {
                var outFiles = new DirectoryInfo(outputPath).GetFiles();
                foreach (var file in FileNameMap)
                {
                    var target = outFiles.FirstOrDefault(f => f.Name == file.Key);
                    target?.Rename(file.Value);
                }
            }
            Engine.Dispose();
            return new HandlerResponse(this, true);
        }

        private ExtendedAppInfo AppInfo { get; set; }

        private AppManifest CreateManifest(string deployDir)
        {
            var mgr = new ManifestManager(string.Empty, deployDir, InformationSource.AppManifest);
            var appManifest = mgr.CreateAppManifest();
            return appManifest;
        }

        private FileInfo GetManifest(string outputPath)
        {
            return new DirectoryInfo(outputPath).GetFiles("*.cltw").FirstOrDefault();
        }

        private FileInfo GetInfoFile(string outputPath)
        {
            return new DirectoryInfo(outputPath).GetFiles("app.info").FirstOrDefault();
        }

        internal AppManifest Manifest { get; set; }

        public Dictionary<string, string> FileNameMap { get; set; } = new Dictionary<string, string>();
    }
}
