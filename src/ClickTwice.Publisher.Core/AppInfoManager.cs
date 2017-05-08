using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core.Manifests;
using Newtonsoft.Json;

namespace ClickTwice.Publisher.Core
{
    public class AppInfoManager : Manager
    {
        private ExtendedAppInfo AppInfo { get; } = new ExtendedAppInfo();
        public AppInfoManager(string projectFilePath) : base(projectFilePath)
        {

        }

        public AppInfoManager() : base(string.Empty)
        {
            
        }

        public AppInfoManager(ExtendedAppInfo appInfo) : base(string.Empty)
        {
            this.AppInfo = appInfo;
        }

        public AppInfoManager AddAuthor(string authorName)
        {
            AppInfo.Author.Names.AddIfNotExists(authorName);
            return this;
        }

        public AppInfoManager AddContactDetails(ContactDetails details)
        {
            AppInfo.Author = details;
            return this;
        }

        public AppInfoManager AddInstallationInformation(string information)
        {
            AppInfo.InstallationInformation = information;
            return this;
        }

        public AppInfoManager AddPrerequisites(params string[] prerequisites)
        {
            foreach (var prerequisite in prerequisites)
            {
                AppInfo.PrerequisiteInformation.Add(prerequisite);
            }
            return this;
        }

        public AppInfoManager AddSupportInformation(string supportInfo)
        {
            AppInfo.SupportInformation = supportInfo;
            return this;
        }

        public AppInfoManager AddDeveloperNotice(string devInfo)
        {
            AppInfo.DeveloperInformation = devInfo;
            return this;
        }

        public AppInfoManager AddAppInformation(string appInfo)
        {
            AppInfo.AppInformation = appInfo;
            return this;
        }

        public void DeployAppInformation(string pathToDeploymentDir)
        {
            var j = JsonConvert.SerializeObject(AppInfo, Formatting.Indented);
            var fi = GetInfoFile(pathToDeploymentDir);
            File.WriteAllText(fi.FullName, j);
        }

        private static FileInfo GetInfoFile(string pathToDeploymentDir)
        {
            var di = new DirectoryInfo(pathToDeploymentDir);
            if (!di.Exists) throw new DirectoryNotFoundException($"Could not find deployment directory at {pathToDeploymentDir}");
            if (di.GetFiles("*.application").Any())
            {
                return new FileInfo(Path.Combine(di.FullName, "app.info"));
            }
            throw new FileNotFoundException($"Could not locate application manifest in {di.Name} directory", di.FullName);
        }

        public static ExtendedAppInfo ReadFromFile(string fullPathToInfoFile)
        {
            return JsonConvert.DeserializeObject<ExtendedAppInfo>(File.ReadAllText(fullPathToInfoFile));
        }
    }
}
