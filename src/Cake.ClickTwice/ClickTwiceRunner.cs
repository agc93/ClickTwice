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
    /// <summary>
    /// A wrapper class for manually invoking individual steps of a ClickTwice build pipeline
    /// </summary>
    public class ClickTwiceRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickTwiceRunner"/> class
        /// </summary>
        /// <param name="log">The Cake log</param>
        /// <param name="environment">The Cake environment</param>
        /// <param name="fileSystem">The Cake filesystem</param>
        /// <param name="processRunner">The Cake process runner</param>
        /// <param name="tools">The Cake tool locator</param>
        public ClickTwiceRunner(ICakeLog log, ICakeEnvironment environment, IFileSystem fileSystem,
            IProcessRunner processRunner, IToolLocator tools)
        {
            Environment = environment;
            FileSystem = fileSystem;
            Log = log;
        }

        private ICakeLog Log { get; set; }

        private IFileSystem FileSystem { get; set; }

        internal ICakeEnvironment Environment { get; set; }

        /// <summary>
        /// Runs the given input handlers on the project at the given path
        /// </summary>
        /// <param name="projectFilePath">Path to the project file</param>
        /// <param name="inputHandlers">A collection of handlers to process</param>
        public void RunInputHandlers(string projectFilePath, params IInputHandler[] inputHandlers)
        {
            inputHandlers.ProcessHandlers(
                FileSystem.GetFile(projectFilePath).Path.GetDirectory().MakeAbsolute(Environment).FullPath,
                s => Log.Information(s));
        }

        /// <summary>
        /// Generates a ClickTwice manifest for the application at the given project path
        /// </summary>
        /// <param name="projectFilePath">Path to the project file</param>
        /// <param name="source">Not currently used.</param>
        /// <returns>A publisher for the generated manifest</returns>
        public ManifestPublisher GenerateManifest(string projectFilePath,
            InformationSource source = InformationSource.Both)
        {
            return new ManifestPublisher(this, projectFilePath);
        }

        /// <summary>
        /// Runs the given output handlers on the published app at the given path
        /// </summary>
        /// <param name="publishDirectoryPath">Path to the published app files</param>
        /// <param name="outputHandlers">A collection of <see cref="IHandler"/> handlers to process</param>
        public void RunOutputHandlers(string publishDirectoryPath, params IOutputHandler[] outputHandlers)
        {
            outputHandlers.ProcessHandlers(
                FileSystem.GetDirectory(publishDirectoryPath).Path.MakeAbsolute(Environment).FullPath,
                s => Log.Information(s));
        }

        /// <summary>
        /// Prepares a directory-based ClickTwice template, using the provided metadata
        /// </summary>
        /// <param name="templateDirectory">Path to the tempalte directory</param>
        /// <param name="packageId">Id of the package to generate</param>
        /// <param name="version">Version of the generated package</param>
        /// <param name="author">Author of the generated package</param>
        /// <param name="description">Optional description of the generated package</param>
        /// <returns></returns>
        public CakeTemplatePublisher PublishTemplate(DirectoryPath templateDirectory, string packageId, string version, string author, string description = null)
        {
            var s = new TemplatePackageSettings()
            {
                Id = packageId,
                Version = "0.0.1",
                Authors = new List<string>() { author },
                Description = description
            };
            return new CakeTemplatePublisher(templateDirectory.MakeAbsolute(Environment).FullPath, s);
        }

        /// <summary>
        /// Prepares a directory-based ClickTwice template, using the provided metadata
        /// </summary>
        /// <param name="templateDirectory">Path to the tempalte directory</param>
        /// <param name="configure">Action for configuring template package settings</param>
        /// <returns></returns>
        public CakeTemplatePublisher PublishTemplate(DirectoryPath templateDirectory,
            Action<TemplatePackageSettings> configure)
        {
            var s = new TemplatePackageSettings();
            configure?.Invoke(s);
            return new CakeTemplatePublisher(templateDirectory.MakeAbsolute(Environment).FullPath, s);
        }
    }
}