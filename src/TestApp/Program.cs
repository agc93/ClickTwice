using ClickTwice.Publisher.Core;
using System;
using System.Linq;

namespace TestApp
{
    internal class Program
    {
        private static string DefaultProjectPath { get; set; } =
            @"C:\Users\alist_000\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj";

        private static void Main(string[] args)
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
            // ReSharper disable once RedundantArgumentDefaultValue
            var result = mgr.PublishApp(null, behaviour: PublishBehaviour.CleanFirst);
            Console.WriteLine(result.Select(r => $"{r.Handler.Name} - {r.Result} - {r.ResultMessage}" + Environment.NewLine));
        }
    }
}