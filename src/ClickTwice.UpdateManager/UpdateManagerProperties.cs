using System.Deployment.Application;

namespace ClickTwice.UpdateManager
{
    public partial class UpdateManagerViewModel
    {
        private bool _isUpdateAvailable;
        private bool _isUpdateInProgress;
        private int _updateProgress;
        private string _updateProgressMessage;

        public bool IsDeployed => ApplicationDeployment.IsNetworkDeployed;

        public bool IsUpdateAvailable
        {
            get { return _isUpdateAvailable; }
            set
            {
                if (value == _isUpdateAvailable) return;
                _isUpdateAvailable = value;
                OnPropertyChanged();
            }
        }

        public bool IsUpdateInProgress
        {
            get { return _isUpdateInProgress; }
            set
            {
                if (value == _isUpdateInProgress) return;
                _isUpdateInProgress = value;
                OnPropertyChanged();
            }
        }

        #region Progress
        public int UpdateProgress
        {
            get { return _updateProgress; }
            set
            {
                if (value == _updateProgress) return;
                _updateProgress = value;
                OnPropertyChanged();
            }
        }

        public string UpdateProgressMessage
        {
            get { return _updateProgressMessage; }
            set
            {
                if (value == _updateProgressMessage) return;
                _updateProgressMessage = value;
                OnPropertyChanged();
            }
        } 
        #endregion
    }
}
