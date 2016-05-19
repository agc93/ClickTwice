using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace ClickTwice.Publisher.Core.Loggers
{
    public class FileLogger : IPublishLogger
    {
        private List<string> Messages { get; set; } = new List<string>();
        public FileLogger(bool includeBuildMessages = true)
        {
            IncludeBuildMessages = includeBuildMessages;
        }
        public void Log(string content)
        {
            Messages.Add(content);
        }

        public bool IncludeBuildMessages { get; set; }
        public string Close(string outputPath)
        {
            var path = Path.Combine(outputPath, $"BuildLog_{DateTime.Now.ToString("YYMMdd-HHmmss")}.txt");
            File.WriteAllLines(path, Messages);
            return $"Log file written to {path}";
        }
    }
}
