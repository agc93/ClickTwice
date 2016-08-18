using System;
using Cake.Core.Diagnostics;
using ClickTwice.Publisher.Core.Loggers;

namespace Cake.ClickTwice
{
    /// <summary>
    /// ClickTwice logger for writing messages to the Cake log
    /// </summary>
    public class CakeLogger : IPublishLogger
    {
        public CakeLogger(ICakeLog log)
        {
            CakeLog = log;
        }

        private ICakeLog CakeLog { get; set; }

        public void Log(string content)
        {
            CakeLog.Information(content);
        }

        public bool IncludeBuildMessages => false;

        public string Close(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}