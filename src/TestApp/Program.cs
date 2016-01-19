using ClickTwice.Publisher.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core.Handlers;

namespace TestApp
{
    internal class Program
    {
        private static string DefaultProjectPath { get; set; } =
            //@"C:\Users\UCRM4\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // ATO
            //@"C:\Users\alist_000\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; //others
            //@"C:\Users\alist\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // Zenbook
            @"C:\Users\alist_000\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";

        private static void Main(string[] args)
        {
            if (args.Any())
            {
                DefaultProjectPath = args.First();
            }
            var mgr = new PublishManager(DefaultProjectPath)
            {
                Platform = "AnyCPU",
                Configuration = "Debug",
                InputHandlers = new List<IInputHandler> {new MockInputHandler()},
                OutputHandlers = new List<IOutputHandler> {new MockOutputHandler()}
            };
            // ReSharper disable once RedundantArgumentDefaultValue
            var path = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            var result = mgr.PublishApp(path.FullName, behaviour: PublishBehaviour.CleanFirst);
            var manager = new ManifestManager(DefaultProjectPath, path.FullName, InformationSource.Both);
            var manifest = manager.CreateAppManifest();
            var cltw = manager.DeployManifest(manifest);
            Console.WriteLine(result.Select(r => $"{r.Handler.Name} - {r.Result} - {r.ResultMessage}" + Environment.NewLine));
        }
    }
}