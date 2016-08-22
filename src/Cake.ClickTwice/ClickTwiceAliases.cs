using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using ClickTwice.Publisher.Core;

namespace Cake.ClickTwice
{
    [CakeAliasCategory("ClickOnce")]
    [CakeNamespaceImport("ClickTwice.Templating")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
    [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
    public static class ClickTwiceAliases
    {
        [CakeNamespaceImport("ClickTwice.Templating")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
        [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
        [CakeMethodAlias]
        public static ClickTwiceManager PublishApp(this ICakeContext ctx, FilePath projectFile)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (ctx.Environment.IsUnix()) throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            return new ClickTwiceManager(projectFile.FullPath, ctx.Log, ctx.Environment, ctx.FileSystem, ctx.ProcessRunner, ctx.Tools);
        }

        [CakeMethodAlias]
        [CakeNamespaceImport("ClickTwice.Templating")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
        [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
        public static AppInfoManager AppInfo(this ICakeContext ctx, FilePath projectFile)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));
            if (ctx.Environment.IsUnix()) throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            var infoPath = projectFile.GetDirectory().GetFilePath("app.info");
            return ctx.FileSystem.Exist(infoPath)
                ? new AppInfoManager(AppInfoManager.ReadFromFile(infoPath.MakeAbsolute(ctx.Environment).FullPath))
                : new AppInfoManager(projectFile.MakeAbsolute(ctx.Environment).FullPath);
        }

        [CakePropertyAlias]
        [CakeNamespaceImport("ClickTwice.Templating")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
        [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
        public static ClickTwiceRunner ClickTwice(this ICakeContext ctx)
        {
            if (ctx.Environment.IsUnix()) throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            return new ClickTwiceRunner(ctx.Log, ctx.Environment, ctx.FileSystem, ctx.ProcessRunner, ctx.Tools);
        }
    }

    public class ManifestPublisher
    {
        internal ManifestPublisher(ClickTwiceRunner runner, string projectFilePath)
        {
            Runner = runner;
            ProjectFilePath = new FilePath(projectFilePath).MakeAbsolute(Runner.Environment);
        }

        private FilePath ProjectFilePath { get; set; }

        private ClickTwiceRunner Runner { get; set; }

        public void Publish(string outputPath, InformationSource source = InformationSource.Both)
        {
            var mgr = new ManifestManager(ProjectFilePath.FullPath, outputPath, source);
            mgr.DeployManifest(mgr.CreateAppManifest());
        }
    }
}

