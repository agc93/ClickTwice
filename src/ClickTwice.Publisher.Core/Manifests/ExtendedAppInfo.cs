using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core.Manifests
{
    public class ExtendedAppInfo
    {
        public ExtendedAppInfo()
        {
            Links = new LinkList();
        }
        public string AppInformation { get; set; } = string.Empty;
        public string InstallationInformation { get; set; } = string.Empty;
        public IList<string> PrerequisiteInformation { get; set; } = new List<string>();
        public ContactDetails Author { get; set; } = new ContactDetails();
        public string SupportInformation { get; set; } = string.Empty;
        public string DeveloperInformation { get; set; } = string.Empty;
        public LinkList Links { get; set; }
    }

    public class LinkList
    {
        public Uri SupportUrl { get; set; } = new Uri("about:blank");
        public Uri DocumentationUri { get; set; } = new Uri("about:blank");
        public Uri DeveloperDocumentation { get; set; } = new Uri("about:blank");
    }

    public class ContactDetails
    {
        public IEnumerable<string> Names { get; set; } = new List<string>();
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        //public string Times { get; set; } = string.Empty;
    }
}
