using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core.Manifests
{
    public class AppManifest
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
        public string SuiteName { get; set; } = string.Empty;
        [Obsolete("Use an ExtendedAppInfo object (or the AppInfoManager) to store application information")]
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Copyright { get; set; } = string.Empty;
        public Version FrameworkVersion { get; set; } = new Version(0,0);
        public Version AppVersion { get; set; } = new Version(0,0);
        public IEnumerable<string> Prerequisites { get; set; } = new List<string>();
    }
}
