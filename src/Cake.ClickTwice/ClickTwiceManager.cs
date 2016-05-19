using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace Cake.ClickTwice
{
    public class ClickTwiceManager
    {
        internal ClickTwiceManager(FilePath projectFile, ICakeContext ctx)
        {
            ProjectFilePath = projectFile;
            Context = ctx;
        }

        private ICakeContext Context { get; set; }

        private FilePath ProjectFilePath { get; set; }

        internal string Platform { private get; set; } = "AnyCPU";
        internal string Configuration { private get; set; } = "Release";

        internal List<IInputHandler> InputHandlers { get; set; } = new List<IInputHandler>();
        internal List<IOutputHandler> OutputHandlers { get; set; } = new List<IOutputHandler>();
        internal List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();
        internal bool CleanOutput { get; set; }
        internal bool ThrowOnHandlerFailure { get; set; }

        private bool AppInfoSupported
            =>
                InputHandlers.Any(
                    h => h.GetType() == typeof(AppInfoHandler));

        public void PublishTo(DirectoryPath outputDirectory)
        {
            OutputHandlers.Add(new PublishPageHandler());
            Loggers.Add(new CakeLogger(Context.Log));
            var mgr = new PublishManager(ProjectFilePath.FullPath, AppInfoSupported ? InformationSource.Both : InformationSource.AssemblyInfo)
            {
                CleanOutputOnCompletion = CleanOutput,
                Configuration = Configuration,
                Platform = Platform,
                InputHandlers = InputHandlers,
                OutputHandlers = OutputHandlers,
                Loggers = Loggers,
            };
            var responses = mgr.PublishApp(outputDirectory.FullPath, PublishBehaviour.DoNotBuild);
            if (ThrowOnHandlerFailure)
            {
                if (responses.Any(r => r.Result == HandlerResult.Error))
                {
                    throw new PublishException(responses.Where(r => r.Result == HandlerResult.Error));
                }
            }
            foreach (var r in responses)
            {
                Context.Log.Information($"Handler finished: {r.Result} - {r.ResultMessage}");
            }
        }

    }
}
