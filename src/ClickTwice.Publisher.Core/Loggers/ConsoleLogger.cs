using System;

namespace ClickTwice.Publisher.Core.Loggers
{
    public class ConsoleLogger : IPublishLogger
    {
        public ConsoleLogger(bool includeBuildMessages = false)
        {
            IncludeBuildMessages = includeBuildMessages;
        }
        public void Log(string content)
        {
            Console.WriteLine(content);
        }

        public bool IncludeBuildMessages { get; set; }
        public string Close(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
