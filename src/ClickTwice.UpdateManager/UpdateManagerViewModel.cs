using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Text;
using MvvmTools;

namespace ClickTwice.UpdateManager
{
    public partial class UpdateManagerViewModel : ViewModelBase
    {

        public UpdateManagerViewModel()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ApplicationDeployment.CurrentDeployment != null)
                {
                    ApplicationDeployment.CurrentDeployment.UpdateCompleted += ApplicationUpdateCompleted;
                    ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += ApplicationUpdateProgressChanged;
                    ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += ApplicationUpdateCheckCompleted;
                }
            }
        }
        
        private void CheckForUpdates(object o)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
            }
        }

        private void UpdateApp(object o)
        {
            if (IsDeployed && IsUpdateAvailable && !IsUpdateInProgress)
            {
                OnUpdateStarting();
                ApplicationDeployment.CurrentDeployment.UpdateAsync();
                IsUpdateInProgress = true;
            }
        }

        private KeyValuePair<string, string> GetAppNameAndBuildDate()
        {
            StringBuilder buildDate = new StringBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDetails = assembly.GetName();
            var name = (ApplicationName == null)
                ? assemblyDetails.FullName
                : ApplicationName();
            if (BuildDate == null)
            {
                try
                {
                    var modDate = new FileInfo(assembly.Location).CreationTimeUtc.ToString("g");
                    buildDate.Append(buildDate);
                }
                catch
                {
                    buildDate.Append("unknown date");
                }
            }
            else
            {
                buildDate = new StringBuilder(BuildDate());
            }
            return new KeyValuePair<string, string>(name, buildDate.ToString());
        }

        private Uri GetAppSourceLocation()
        {
            Uri source;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                source = ApplicationDeployment.CurrentDeployment.ActivationUri ??
                         ApplicationDeployment.CurrentDeployment.UpdateLocation;
            }
            else
            {
                var assembly = Assembly.GetExecutingAssembly();
                source = new Uri(assembly.Location);
            }
            return source;
        }

        public string ApplicationInfoMessage
        {
            get
            {
                var details = GetAppNameAndBuildDate();
                var s =
                    $"{details.Key} version {Assembly.GetExecutingAssembly().GetName().Version.ToString(4)} built {details.Value}";
                return s;
            } }
                

        public string DeploymentInfoMessage => $"Deployment version {ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4)} deployed from {GetAppSourceLocation()?.Host ?? "unknown location"}";

        public string UpdateCheckStatusMessage
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    var firstRun = ApplicationDeployment.CurrentDeployment.IsFirstRun ? " (first run)" : string.Empty;
                    return $"Updates check from {ApplicationDeployment.CurrentDeployment?.UpdateLocation?.Host} last completed {ApplicationDeployment.CurrentDeployment.TimeOfLastUpdateCheck.ToString("f")}{firstRun}.";
                }
                return string.Empty;
            }
        }

        private void ApplicationUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs deploymentProgressChangedEventArgs)
        {
            var a = deploymentProgressChangedEventArgs;
            UpdateProgress = a.ProgressPercentage;
            UpdateProgressMessage = a.State.ToString().SplitCamelCase();
        }

        private void ApplicationUpdateCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            IsUpdateInProgress = false;
            OnUpdateComplete();
        }

        private void ApplicationUpdateCheckCompleted(object sender, CheckForUpdateCompletedEventArgs checkForUpdateCompletedEventArgs)
        {
            var info = checkForUpdateCompletedEventArgs;
            IsUpdateAvailable = info.UpdateAvailable;
            if (info.UpdateAvailable)
            {
                UpdateInfo = new UpdateInfo
                {
                    DownloadSize = new DownloadSize(info.UpdateSizeBytes),
                    Mandatory = info.IsUpdateRequired,
                    MinimumVersion = info.MinimumRequiredVersion,
                    NewVersion = info.AvailableVersion,
                    UpdateSource = ApplicationDeployment.CurrentDeployment?.UpdateLocation
                };
            }
            else
            {
                UpdateInfo = null;
            }
            OnPropertyChanged(nameof(UpdateInfo));
        }

        public UpdateInfo UpdateInfo { get; private set; }

        

        

        public Func<string> ApplicationName { get; set; } 
        public Func<string> BuildDate { get; set; }
    }
}
