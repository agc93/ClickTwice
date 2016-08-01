using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.MSBuild;
using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
    public class ClickTwicePack : IScriptPackContext
    {
        public ClickTwicePack()
        {
            
        }

        public Publisher PublishApp(string projectFilePath)
        {
            var mgr = new PublishManager(projectFilePath, InformationSource.Both)
            {
                Configuration = Settings.Configuration,
                Platform = Settings.Platform,
                CleanOutputOnCompletion = Settings.OutputClean,
                InputHandlers = Settings.InputHandlers,
                OutputHandlers =  Settings.OutputHandlers,
                Loggers = Settings.Loggers
            };
            return new Publisher(mgr);
        }

        public void Configure(Action<ClickTwicePackSettings> configure)
        {
            configure.Invoke(Settings);
        }

        private ClickTwicePackSettings Settings { get; set; } = new ClickTwicePackSettings();
    }
}
