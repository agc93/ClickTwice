using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Resources;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class PublishPageHandler : IOutputHandler
    {
        public string Name => "publish.htm Generator";
        public HandlerResponse Process(string outputPath)
        {
            try
            {
                var page = new PublishPage(outputPath);
                page.CreatePage();
                if (new DirectoryInfo(outputPath).GetFiles("publish.htm").Any())
                {
                    return new HandlerResponse(this, true, "publish.htm successfully generated in " + outputPath);
                }
                return new HandlerResponse(this, false,
                    $"publish.htm was not generated. Check to ensure a ClickTwice manifest has been deployed in the target directory");
            }
            catch
            {
                return new HandlerResponse(this, false, $"Error encountered while generating publish.htm page in {outputPath}. Check your deployment files and try again.");
            }
        }
    }
}
