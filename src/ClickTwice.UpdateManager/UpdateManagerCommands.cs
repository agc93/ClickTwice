using System.Windows.Input;
using MvvmTools;

namespace ClickTwice.UpdateManager
{
    public partial class UpdateManagerViewModel
    {
        public ICommand UpdateApplicationCommand => new RelayCommand(UpdateApp, o => IsUpdateAvailable);
        public ICommand CheckUpdatesCommand => new RelayCommand(CheckForUpdates, o => IsDeployed);

    }
}
