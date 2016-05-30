using System;
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
        public ClickTwiceManager(FilePath projectFile, ICakeLog log, ICakeEnvironment environment, IFileSystem fs, IProcessRunner runner, IGlobber globber)
        {
            Log = log;
            Environment = environment;
            FileSystem = fs;
            ProcessRunner = runner;
            Globber = globber;
            ProjectFilePath = projectFile.MakeAbsolute(Environment).FullPath;
        }

        private ICakeEnvironment Environment { get; set; }

        private ICakeLog Log { get; set; }
        private IFileSystem FileSystem { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IGlobber Globber { get; set; }

        private string ProjectFilePath { get; set; }

        internal string Platform { private get; set; } = "AnyCPU";
        internal string Configuration { private get; set; } = "Release";

        internal List<IInputHandler> InputHandlers { get; set; } = new List<IInputHandler>();
        internal List<IOutputHandler> OutputHandlers { get; set; } = new List<IOutputHandler>();
        internal List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();
        internal bool CleanOutput { get; set; }
        internal bool ThrowOnHandlerFailure { get; set; }
        internal bool ForceBuild { get; set; }

        internal string PublishVersion { get; set; }

        internal Action<CakePublishManager> BuildAction { private get; set; }

        private bool AppInfoSupported
            =>
                InputHandlers.Any(
                    h => h.GetType() == typeof(AppInfoHandler));

        public void PublishTo(DirectoryPath outputDirectory)
        {
            OutputHandlers.Add(new PublishPageHandler());
            Loggers.Add(new CakeLogger(Log));
            var mgr = new CakePublishManager(Environment, FileSystem, ProcessRunner, Globber, ProjectFilePath, AppInfoSupported ? InformationSource.Both : InformationSource.AssemblyInfo)
            {
                CleanOutputOnCompletion = CleanOutput,
                Configuration = Configuration,
                Platform = Platform,
                InputHandlers = InputHandlers,
                OutputHandlers = OutputHandlers,
                BuildAction = BuildAction
            };
            if (!string.IsNullOrWhiteSpace(PublishVersion)) mgr.AdditionalProperties.Add("ApplicationVersion", PublishVersion);
            var responses = mgr.PublishApp(outputDirectory.MakeAbsolute(Environment).FullPath, ForceBuild ? PublishBehaviour.CleanFirst : PublishBehaviour.DoNotBuild);
            if (ThrowOnHandlerFailure)
            {
                if (responses.Any(r => r.Result == HandlerResult.Error))
                {
                    throw new PublishException(responses.Where(r => r.Result == HandlerResult.Error));
                }
            }
            foreach (var r in responses)
            {
                Log.Information($"Handler finished: {r.Result} - {r.ResultMessage}");
            }
        }

    }
}
