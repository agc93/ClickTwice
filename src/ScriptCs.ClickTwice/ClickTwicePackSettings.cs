using System.Collections.Generic;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace ScriptCs.ClickTwice
{
    public class ClickTwicePackSettings
    {
        public ClickTwicePackSettings WithHandler(IInputHandler handler)
        {
            InputHandlers.Add(handler);
            return this;
        }

        public ClickTwicePackSettings WithHandler(IOutputHandler handler)
        {
            OutputHandlers.Add(handler);
            return this;
        }

        public ClickTwicePackSettings WithLogger(IPublishLogger logger)
        {
            Loggers.Add(logger);
            return this;
        }

        internal List<IPublishLogger> Loggers { get; set; } = new List<IPublishLogger>();

        internal List<IOutputHandler> OutputHandlers { get; set; } = new List<IOutputHandler>();

        internal List<IInputHandler> InputHandlers { get; set; } = new List<IInputHandler>();

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

        public ClickTwicePackSettings EnableCleanFirst()
        {
            CleanFirst = true;
            return this;
        }

        public ClickTwicePackSettings CleanOutputDirectory()
        {
            OutputClean = true;
            return this;
        }

        internal bool OutputClean { get; set; }

        internal bool CleanFirst { get; set; }

        internal string Platform { get; set; } = "AnyCPU";

        internal string Configuration { get; set; } = "Release";
    }
}