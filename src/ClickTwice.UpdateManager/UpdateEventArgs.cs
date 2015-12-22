using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.UpdateManager
{
    public class UpdateEventArgs : EventArgs
    {
        public Version PreviousVersion { get; set; }
        public Version NewVersion { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
    }
}
