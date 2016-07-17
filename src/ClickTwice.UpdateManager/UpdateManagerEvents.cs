using System.Reflection;

namespace ClickTwice.UpdateManager
{
    public partial class UpdateManagerViewModel
    {

        public event UpdateHandler UpdateCompleted;
        public event UpdateHandler UpdateStarting;
        private UpdateEventArgs e = null;

        public delegate void UpdateHandler(UpdateManagerViewModel m, UpdateEventArgs e);

        protected virtual void OnUpdateComplete()
        {
            e = new UpdateEventArgs
            {
                PreviousVersion = Assembly.GetExecutingAssembly().GetName().Version,
                NewVersion = UpdateInfo.NewVersion,
                UpdateInfo = UpdateInfo
            };
            UpdateCompleted?.Invoke(this, e);
        }

        protected virtual void OnUpdateStarting()
        {
            e = new UpdateEventArgs
            {
                PreviousVersion = Assembly.GetExecutingAssembly().GetName().Version,
                NewVersion = UpdateInfo.NewVersion,
                UpdateInfo = UpdateInfo
            };
            UpdateStarting?.Invoke(this, e);
        }
    }
}
