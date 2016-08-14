using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core.Resources;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class InstallPageHandler : IOutputHandler
    {
        public InstallPageHandler(string fileName = "index.htm", string linkText = null, string linkTarget = null)
        {
            this.OutputFileName = fileName;
            AdditionalLink = new KeyValuePair<string, string>(linkText, linkTarget);
            UseLink = !string.IsNullOrWhiteSpace(linkTarget) && !string.IsNullOrWhiteSpace(linkText);
        }

        private bool UseLink { get; set; }

        private KeyValuePair<string, string> AdditionalLink { get; set; }

        public string OutputFileName { get; set; }

        public string Name => "Improved application launch page generator";
        public HandlerResponse Process(string outputPath)
        {
            try
            {
                var page = new LaunchPage(outputPath, OutputFileName);
                if (UseLink) page.AdditionalLink = AdditionalLink;
                page.CreatePage();
                if (new DirectoryInfo(outputPath).GetFiles(OutputFileName).Any())
                {
                    return new HandlerResponse(this, true, $"{OutputFileName} successfully generated in " + outputPath);
                }
                return new HandlerResponse(this, false,
                    $"Launch page was not generated. Check to ensure a ClickTwice manifest has been deployed in the target directory");
            }
            catch (System.Exception ex)
            {
                return new HandlerResponse(this, false, $"{ex.GetType()} error encountered while generating launch page in {outputPath}. Check your deployment files and try again. {System.Environment.NewLine} {ex.Message}");
            }
        }
    }
}
