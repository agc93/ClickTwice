using System;
using System.Linq;
using System.Reflection;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Loggers;
using ClickTwice.Publisher.MSBuild;
using Microsoft.VisualBasic.Logging;
using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
    public class ClickTwicePack : IScriptPackContext
    {
        public Publisher PublishApp(string projectFilePath, Action<ClickTwicePackSettings> configure = null) {
            
            configure?.Invoke(Settings);
            var host = new ConsoleScriptHost();
            BasePublishManager mgr;
            if (Settings.UseDirectPublish)
            {
                mgr = new DirectPublisher(projectFilePath, ToSource(Settings));
            }
            else
            {
                mgr = new PublishManager(projectFilePath, ToSource(Settings));
            }
            mgr.Configuration = Settings.Configuration;
            mgr.Platform = Settings.Platform;
            mgr.CleanOutputOnCompletion = Settings.OutputClean;
            mgr.InputHandlers = Settings.InputHandlers;
            mgr.OutputHandlers = Settings.OutputHandlers;
            mgr.Loggers = Settings.Loggers;
            mgr.Loggers.RemoveAll(l => l.GetType() == typeof(ConsoleLogger));
            mgr.Loggers.Add(new ConsoleLogger(Settings.LogBuildMessages));
            return new Publisher(mgr) {Host = host};
        }

        public void Configure(Action<ClickTwicePackSettings> configure)
        {
            configure.Invoke(Settings);
        }

        private ClickTwicePackSettings Settings { get; set; } = new ClickTwicePackSettings();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static InformationSource ToSource(ClickTwicePackSettings settings)
        {
            if (settings.UseAppManifest && settings.UseAssemblyInfo) return InformationSource.Both;
            if (settings.UseAppManifest) return InformationSource.AppManifest;
            if (settings.UseAssemblyInfo) return InformationSource.AssemblyInfo;
            return InformationSource.None;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',').FirstOrDefault();
            Console.WriteLine($"Resolving {assemblyName}");
            return assemblyName == null ? Assembly.Load(args.Name) : Assembly.Load(assemblyName);
        }
    }
}
