using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Handlers.AppDetailsPage
{
    public class AppDetailsPageHandler : IOutputHandler
    {
        public AppDetailsPageHandler(string templateName)
        {
            //NuGet-related template extraction goes here
            var packageEngine = new PackageEngine(templateName);
            packageEngine.ExtractPackage();
        }

        public AppDetailsPageHandler(FileInfo localPackage)
        {
            var packageEngine = new PackageEngine(localPackage);
            var extractPackage = packageEngine.ExtractPackage();
            TemplateDirectory = extractPackage;
        }

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
                throw new InvalidOperationException("This handler requires both a deployment manifest (cltw file) and an info file (app.info)! Try adding AppInfoHandler to your OutputHandlers and ensure you have set an InformationSource to generate a manifest.") ;
            }
            Manifest = ManifestManager.ReadFromFile(GetManifest(outputPath).FullName);
            AppInfo = AppInfoManager.ReadFromFile(GetInfoFile(outputPath).FullName);

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

        public AppManifest Manifest { get; set; }
    }
}
