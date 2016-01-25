using ClickTwice.Publisher.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClickTwice.Handlers.LaunchPage;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace TestApp
{
    internal class Program
    {
        private static string DefaultProjectPath { get; set; } =
        //@"C:\Users\UCRM4\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // ATO
        //@"C:\Users\alist_000\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; //others
        //@"C:\Users\alist\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // Zenbook
        //@"C:\Users\alist_000\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";
        //@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";
        @"C:\Users\UCRM4\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";

        private static void Main(string[] args)
        {
            if (args.Any())
            {
                DefaultProjectPath = args.First();
            }
            var log = new ConsoleLogger();
            var file = new FileLogger();
            var info = new AppInfoManager();
            var mgr = new PublishManager(DefaultProjectPath, InformationSource.Both)
            {
                Platform = "AnyCPU",
                Configuration = "Debug",
                InputHandlers = new List<IInputHandler> {new MockInputHandler()},
                OutputHandlers = new List<IOutputHandler> {new AppInfoHandler(info), new PublishPageHandler(), new InstallPageHandler("index.htm") },
                Loggers = new List<IPublishLogger> { log, file }
            };
            // ReSharper disable once RedundantArgumentDefaultValue
            var path = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            var result = mgr.PublishApp(path.FullName, behaviour: PublishBehaviour.CleanFirst);
            //var manager = new ManifestManager(DefaultProjectPath, path.FullName, InformationSource.Both);
            //var manifest = manager.CreateAppManifest();
            //var cltw = manager.DeployManifest(manifest);
            Process.Start(path.FullName);
            Console.WriteLine(result.Select(r => $"{r.Handler.Name} - {r.Result} - {r.ResultMessage}" + Environment.NewLine));
        }
    }
}