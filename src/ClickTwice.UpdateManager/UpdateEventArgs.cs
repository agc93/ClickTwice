using System;

namespace ClickTwice.UpdateManager
{
    public class UpdateEventArgs : EventArgs
    {
        public Version PreviousVersion { get; set; }
        public Version NewVersion { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
    }
}
