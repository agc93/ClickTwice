using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Templating;

namespace Cake.ClickTwice
{
    public class ClickTwiceRunner
    {
        public ClickTwiceRunner(ICakeLog log, ICakeEnvironment environment, IFileSystem fileSystem, IProcessRunner processRunner, IToolLocator tools)
        {
            Environment = environment;
            FileSystem = fileSystem;
            Log = log;
        }

        private ICakeLog Log { get; set; }

        private IFileSystem FileSystem { get; set; }

        internal ICakeEnvironment Environment { get; set; }

        public void RunInputHandlers(string projectFilePath, params IInputHandler[] inputHandlers)
        {
            inputHandlers.ProcessHandlers(
                FileSystem.GetFile(projectFilePath).Path.GetDirectory().MakeAbsolute(Environment).FullPath,
                s => Log.Information(s));
        }

        public ManifestPublisher GenerateManifest(string projectFilePath, InformationSource source = InformationSource.Both)
        {
            return new ManifestPublisher(this, projectFilePath);
            //return new ManifestManager(new FilePath(projectFilePath).MakeAbsolute(Environment).FullPath, string.Empty, source).CreateAppManifest();
        }

        public void RunOutputHandlers(string publishDirectoryPath, params IOutputHandler[] outputHandlers)
        {
            outputHandlers.ProcessHandlers(
                FileSystem.GetDirectory(publishDirectoryPath).Path.MakeAbsolute(Environment).FullPath,
                s => Log.Information(s));
        }

        public CakeTemplatePublisher PublishTemplate(DirectoryPath templateDirectory, string packageId, string version, string author, string description = null)
        {
            var s = new TemplatePackageSettings()
            {
                Id = packageId,
                Version = "0.0.1",
                Authors = new List<string>() {author},
                Description = description
            };
            return new CakeTemplatePublisher(templateDirectory.MakeAbsolute(Environment).FullPath, s);
        }

        public CakeTemplatePublisher PublishTemplate(DirectoryPath templateDirectory,
            Action<TemplatePackageSettings> configure)
        {
            var s = new TemplatePackageSettings();
            configure?.Invoke(s);
            return new CakeTemplatePublisher(templateDirectory.MakeAbsolute(Environment).FullPath, s);
        }
    }
}