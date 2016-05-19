using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class AppInfoHandler : IInputHandler, IOutputHandler
    {
        public AppInfoHandler(AppInfoManager manager = null)
        {
            Manager = manager;
        }

        private AppInfoManager Manager { get; set; }

        public string Name => "app.info file handler";
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
            {
                if (Manager != null)
                {
                    Manager.DeployAppInformation(inputPath);
                    AppInfo = AppInfoManager.ReadFromFile(Path.Combine(inputPath, "app.info"));
                }
                else
                {
                    return new HandlerResponse(this, HandlerResult.NotRun, $"No app.info file found in '{inputPath}'");
                }
            }
            else
            {
                AppInfo = AppInfoManager.ReadFromFile(files.First().FullName);
            }
            return new HandlerResponse(this, true);
        }

        private ExtendedAppInfo AppInfo { get; set; }
    }
}
