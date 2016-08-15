using System;
using ClickTwice.Publisher.Core.Loggers;
using Xunit.Abstractions;

namespace Cake.ClickTwice.Tests
{
    public class UnitTestLogger : IPublishLogger
    {
        public UnitTestLogger(ITestOutputHelper helper)
        {
            Logger = helper;
            IncludeBuildMessages = true;
        }

        private ITestOutputHelper Logger { get; set; }

        public void Log(string content)
        {
            Logger.WriteLine(content);
        }

        public bool IncludeBuildMessages { get; set; }
        public string Close(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}