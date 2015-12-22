using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
