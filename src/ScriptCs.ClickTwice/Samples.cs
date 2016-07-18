using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Loggers;

namespace ScriptCs.ClickTwice
{
    class Samples
    {
        private void Sample()
        {
            ClickTwicePack pack = null;
            pack.Configure(s => s.WithLogger(new ConsoleLogger()));
            pack.PublishApp("./path/to/project.proj").To("./artifacts/publish");
        }
    }
}
