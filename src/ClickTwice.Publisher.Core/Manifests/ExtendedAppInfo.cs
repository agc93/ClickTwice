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
        public string InstallationInformation { get; set; } = string.Empty;
        public IList<string> PrerequisiteInformation { get; set; } = new List<string>();
        public ContactDetails Author { get; set; } = new ContactDetails();
        public string SupportInformation { get; set; } = string.Empty;
        public string DeveloperInformation { get; set; } = string.Empty;
    }

    public class ContactDetails
    {
        public IEnumerable<string> Names { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Times { get; set; }
    }
}
