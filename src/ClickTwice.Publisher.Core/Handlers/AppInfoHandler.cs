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
            if (AppInfo != null)
            {
                //we found a manifest at input time
                Manager = new AppInfoManager(AppInfo);
                //there wasn't a manifest when we started
                // so we clearly just created this
                
            }
            //This will either create one using the passed in Manager or the discovered app.info
            Manager?.DeployAppInformation(outputPath);
            return new HandlerResponse(this, di.GetFiles("app.info").Any(), $"Deployed app.info in '{di.Name}' directory");
        }

        HandlerResponse IInputHandler.Process(string inputPath)
        {
            var files = new DirectoryInfo(inputPath).EnumerateFiles("app.info", SearchOption.AllDirectories);
            if (files.Any())
            {
                //return new HandlerResponse(this, HandlerResult.NotRun, $"No app.info file found in '{inputPath}'");
                AppInfo = AppInfoManager.ReadFromFile(files.First().FullName);
            }
            return new HandlerResponse(this, true);
        }

        private ExtendedAppInfo AppInfo { get; set; }
    }
}
