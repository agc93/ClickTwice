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
}
