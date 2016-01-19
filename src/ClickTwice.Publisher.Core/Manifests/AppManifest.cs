using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core.Manifests
{
    public class AppManifest
    {
        public string ApplicationName { get; set; }
        public string ShortName { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public string SuiteName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public Version FrameworkVersion { get; set; }
        public Version AppVersion { get; set; }
        public IEnumerable<string> Prerequisites { get; set; } 
    }
}
