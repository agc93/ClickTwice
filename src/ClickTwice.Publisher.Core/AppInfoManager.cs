using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AddAuthor(string authorName)
        {
            AppInfo.Author.Names.AddIfNotExists(authorName);
        }

        public void AddContactDetails(ContactDetails details)
        {
            AppInfo.Author = details;
        }

        public void AddInstallationInformation(string information)
        {
            AppInfo.InstallationInformation = information;
        }

        public void AddPrerequisites(params string[] prerequisites)
        {
            foreach (var prerequisite in prerequisites)
            {
                AppInfo.PrerequisiteInformation.Add(prerequisite);
            }
        }

        public void AddSupportInformation(string supportInfo)
        {
            AppInfo.SupportInformation = supportInfo;
        }

        public void AddDeveloperNotice(string devInfo)
        {
            AppInfo.DeveloperInformation = devInfo;
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
