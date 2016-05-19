using System;
using Cake.Core.Diagnostics;
using ClickTwice.Publisher.Core.Loggers;

namespace Cake.ClickTwice
{
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

        public bool IncludeBuildMessages
        {
            get { return false; }
            set { }
        }

        public string Close(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}