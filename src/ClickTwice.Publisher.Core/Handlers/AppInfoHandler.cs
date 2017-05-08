using System;
using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class AppInfoHandler : IInputHandler, IOutputHandler
    {
        public AppInfoHandler(AppInfoManager manager = null)
        {
            Manager = manager;
        }

        public AppInfoHandler(Action<AppInfoManager> configure)
        {
            Configuration = configure;
        }

        private Action<AppInfoManager> Configuration { get; set; }

        private AppInfoManager Manager { get; set; }

        public string Name => "app.info file handler";
        HandlerResponse IOutputHandler.Process(string outputPath)
        {
            var di = new DirectoryInfo(outputPath);
            if (AppInfo != null)
            {
                //we found a manifest at input time
                
                //there wasn't a manifest when we started
                // so we clearly just created this
            }
            //This will either create one using the passed in Manager or the discovered app.info
            Manager?.DeployAppInformation(outputPath);
            return new HandlerResponse(this, di.GetFiles("app.info").Any(), $"Deployed app.info in '{di.Name}' directory");
        }

        HandlerResponse IInputHandler.Process(string inputPath)
        {
            var files = new DirectoryInfo(inputPath).EnumerateFiles("app.info", SearchOption.AllDirectories).ToList();
            var projects = new DirectoryInfo(inputPath).EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly);
            if (files.Any())
            {
                AppInfo = AppInfoManager.ReadFromFile(files.First().FullName);
                Manager = new AppInfoManager(AppInfo);
            }
            else
            {
                Manager = new AppInfoManager(projects.FirstOrDefault()?.FullName);
            }
            if (Configuration != null && Manager != null)
            {
                Configuration.Invoke(Manager);
            }
            return new HandlerResponse(this, true);
        }

        private ExtendedAppInfo AppInfo { get; set; }
    }
}
