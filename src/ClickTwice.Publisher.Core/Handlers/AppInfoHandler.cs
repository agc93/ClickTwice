using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class AppInfoHandler : IOutputHandler
    {
        public AppInfoHandler(AppInfoManager manager)
        {
            this.Manager = manager;
        }

        public AppInfoManager Manager { get; set; }

        public AppInfoHandler(ExtendedAppInfo appInfo) : this(new AppInfoManager(appInfo))
        {
            
        }
        public string Name => "Simple handler to generate app.info files";
        public HandlerResponse Process(string outputPath)
        {
            Manager.DeployAppInformation(outputPath);
            try
            {
                var fromFile = AppInfoManager.ReadFromFile(Path.Combine(outputPath, "app.info"));
                return fromFile != null
                    ? new HandlerResponse(this, true, "AppInfo successfully generated")
                    : new HandlerResponse(this, false, "No app.info found in output folder!");
            }
            catch (Exception ex)
            {
                return new HandlerResponse(this, false, $"Error encountered while creating app.info: {ex.Message}");
            }
        }
    }

    public class AppInfoFileHandler : IInputHandler, IOutputHandler
    {
        public string Name => "Handler to use existing app.info files";
        HandlerResponse IOutputHandler.Process(string outputPath)
        {
            var di = new DirectoryInfo(outputPath);
            if (AppInfo == null) return new HandlerResponse(this, HandlerResult.NotRun, "app.info not found, so cannot publish!");
            if (di.GetFiles("app.info").Any())
            {
                return new HandlerResponse(this, false, "Conflicting app.info file found in output directory. Make sure you are not also generating one when publishing!");
            }
            var mgr = new AppInfoManager(AppInfo);
            mgr.DeployAppInformation(outputPath);
            return new HandlerResponse(this, true, $"Deployed app.info in '{di.Name}' directory");
        }

        HandlerResponse IInputHandler.Process(string inputPath)
        {
            var files = new DirectoryInfo(inputPath).EnumerateFiles("app.info", SearchOption.AllDirectories);
            if (!files.Any())
                return new HandlerResponse(this, HandlerResult.NotRun, $"No app.info file found in '{inputPath}'");
            var info = AppInfoManager.ReadFromFile(files.First().FullName);
            this.AppInfo = info;
            return new HandlerResponse(this, true);
        }

        private ExtendedAppInfo AppInfo { get; set; }
    }
}
