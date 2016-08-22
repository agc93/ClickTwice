using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Templating
{
    public class TemplatePackageSettings
    {
        public List<string> Authors { get; set; } = new List<string>();
        public string Description { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }
}
