using Cake.Core.IO;
using ClickTwice.Publisher.Core;

namespace Cake.ClickTwice
{
    /// <summary>
    ///  Convenience class for generating app manifests
    /// </summary>
    public class ManifestPublisher
    {
        internal ManifestPublisher(ClickTwiceRunner runner, string projectFilePath)
        {
            Runner = runner;
            ProjectFilePath = new FilePath(projectFilePath).MakeAbsolute(Runner.Environment);
        }

        private FilePath ProjectFilePath { get; set; }

        private ClickTwiceRunner Runner { get; set; }

        /// <summary>
        /// Publish the generated manifest at the given path
        /// </summary>
        /// <param name="outputPath">Path to the output file</param>
        /// <param name="source">Used to control the sourcing of app metadata</param>
        public void Publish(string outputPath, InformationSource source = InformationSource.Both)
        {
            var mgr = new ManifestManager(ProjectFilePath.FullPath, outputPath, source);
            mgr.DeployManifest(mgr.CreateAppManifest());
        }
    }
}