using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core.Loggers
{
    public interface IPublishLogger
    {
        void Log(string content);
        bool IncludeBuildMessages { get; }
        string Close(string outputPath);
    }
}
