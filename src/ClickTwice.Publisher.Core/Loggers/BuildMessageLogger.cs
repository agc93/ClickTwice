using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace ClickTwice.Publisher.Core.Loggers
{
    internal class BuildMessageLogger : ILogger
    {
        public BuildMessageLogger(IList<IPublishLogger> loggers)
        {
            this.Loggers = loggers;
            Verbosity = LoggerVerbosity.Normal;
        }

        private IList<IPublishLogger> Loggers { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.AnyEventRaised += EventSource_AnyEventRaised;
        }

        private void EventSource_AnyEventRaised(object sender, BuildEventArgs e)
        {
            foreach (var logger in Loggers)
            {
                logger.Log($"{DateTime.Now.ToString("T")} - {e.Message}");
            }
        }

        public void Shutdown()
        {
            return;
        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }
}
