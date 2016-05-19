﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Resources;

namespace ClickTwice.Publisher.Core.Handlers
{
    public class InstallPageHandler : IOutputHandler
    {
        public InstallPageHandler(string fileName)
        {
            this.OutputFileName = fileName;
        }

        public string OutputFileName { get; set; }

        public string Name => "Improved application launch page generator";
        public HandlerResponse Process(string outputPath)
        {
            try
            {
                var page = new LaunchPage(outputPath, OutputFileName);
                page.CreatePage();
                if (new DirectoryInfo(outputPath).GetFiles(OutputFileName).Any())
                {
                    return new HandlerResponse(this, true, $"{OutputFileName} successfully generated in " + outputPath);
                }
                return new HandlerResponse(this, false,
                    $"Launch page was not generated. Check to ensure a ClickTwice manifest has been deployed in the target directory");
            }
            catch
            {
                return new HandlerResponse(this, false, $"Error encountered while generating launch page in {outputPath}. Check your deployment files and try again.");
            }
        }
    }
}
