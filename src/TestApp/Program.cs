using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;

namespace TestApp
{
    class Program
    {
        private static string DefaultProjectPath { get; set; } =
            @"C:\Users\alist\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj";
        static void Main(string[] args)
        {
            if (args.Any())
            {
                DefaultProjectPath = args.First();
            }
            var mgr = new PublishManager(DefaultProjectPath)
            {
                Platform = "AnyCPU",
                Configuration = "Debug"
            };
            var result = mgr.PublishApp(PublishBehaviour.CleanFirst);
            Console.WriteLine(result.OverallResult.ToString());
            Console.WriteLine(result.OverallResult.ToString());
        }
    }
}
