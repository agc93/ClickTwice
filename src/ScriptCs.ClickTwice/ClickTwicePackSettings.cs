using System.Collections.Generic;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace ScriptCs.ClickTwice
{
    public class ClickTwicePackSettings
    {
        public ClickTwicePackSettings WithHandler(IHandler handler)
        {
            var input = handler as IInputHandler;
            var output = handler as IOutputHandler;
            if (input != null)
            {
                InputHandlers.Add(input);
            }
            if (output != null)
            {
                OutputHandlers.Add(output);
            }
            return this;
        }

        public ClickTwicePackSettings WithLogger(IPublishLogger logger)
        {
            Loggers.Add(logger);
            return this;
        }

        public ClickTwicePackSettings SetConfiguration(string configuration)
        {
            this.Configuration = configuration;
            return this;
        }

        public ClickTwicePackSettings SetPlatform(string platform)
        {
            Platform = platform;
            return this;
        }

        public ClickTwicePackSettings PreserveBuildOutput()
        {
            OutputClean = false;
            return this;
        }

        public ClickTwicePackSettings EnableBuildMessages()
        {
            LogBuildMessages = true;
            return this;
        }

        public ClickTwicePackSettings UseLocalMsBuild()
        {
            UseDirectPublish = true;
            return this;
        }

        public ClickTwicePackSettings UseAssemblyInfoMetadata(bool useAssemblyInfo = true)
        {
            UseAssemblyInfo = useAssemblyInfo;
            return this;
        }

        public ClickTwicePackSettings UseAppManifestMetadata(bool useMetadata = true)
        {
            UseAppManifest = useMetadata;
            return this;
        }

        internal bool LogBuildMessages { get; set; }

        internal bool OutputClean { get; set; } = true;

        internal string Platform { get; set; } = "AnyCPU";

        internal string Configuration { get; set; } = "Release";

        internal bool UseDirectPublish { get; set; }

        internal bool UseAssemblyInfo { get; set; }

        internal bool UseAppManifest { get; set; } = true;

        internal List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();

        internal List<IOutputHandler> OutputHandlers { get; set; } = new List<IOutputHandler>();

        internal List<IInputHandler> InputHandlers { get; set; } = new List<IInputHandler>();
    }
}