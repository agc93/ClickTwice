using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Handlers.LaunchPage
{
    public class LaunchPageHandler : IOutputHandler
    {
        public LaunchPageHandler()
        {
            
        }

        private EmbeddedTemplateManager TemplateSource { get; set; }


        public string Name => "Advanced Launch/Install Page Handler";
        public HandlerResponse Process(string outputPath)
        {
            this.InfoManager = new AppInfoManager(string.Empty);
            var di = new DirectoryInfo(outputPath);
            if (!di.Exists) throw new DirectoryNotFoundException("Invalid output path!");
            var files = di.GetFiles();
            var manifestPresent = files.Any(f => f.Extension == ".cltw");
            var infoPresent = files.Any(f => f.Name == "app.info");
            TemplateEngine engine = null;
            if (manifestPresent)
            {
                Manifest = ManifestManager.ReadFromFile(GetManifest(outputPath).FullName);
            }
            else
            {
                Manifest = CreateManifest(outputPath);
                manifestPresent = Manifest != null;
            }
            if (infoPresent)
            {
                AppInfo = AppInfoManager.ReadFromFile(GetInfoFile(outputPath).FullName);
            }
            if (manifestPresent && infoPresent)
            {
                engine = new TemplateEngine(AppInfo, Manifest);
            }
            else
            {
                engine = new TemplateEngine(Manifest);
            }
            var page = engine.BuildPage("Eventually");
            engine.WritePage(page, Path.Combine(outputPath, "index.htm"));
            return new HandlerResponse(this, true);
        }

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

        internal AppInfoManager InfoManager { get; set; }
        internal ExtendedAppInfo AppInfo { get; set; }
        internal AppManifest Manifest { get; set; }
    }
}
