using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using ClickTwice.Publisher.Core;

namespace Cake.ClickTwice
{
    /// <summary>
    /// Aliases for publishing ClickOnce apps using the ClickTwice toolchain
    /// </summary>
    [CakeAliasCategory("ClickOnce")]
    [CakeNamespaceImport("ClickTwice.Templating")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
    [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
    [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
    public static class ClickTwiceAliases
    {
        /// <summary>
        /// Prepares a publish of the given project
        /// </summary>
        /// <param name="ctx">The Cake context</param>
        /// <param name="projectFile">Path to the project file of the app you want to publish</param>
        /// <returns>A manager to publish your app from</returns>
        [CakeNamespaceImport("ClickTwice.Templating")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
        [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
        [CakeMethodAlias]
        public static ClickTwiceManager PublishApp(this ICakeContext ctx, FilePath projectFile)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (ctx.Environment.IsUnix())
                throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            return new ClickTwiceManager(projectFile.FullPath, ctx.Log, ctx.Environment, ctx.FileSystem,
                ctx.ProcessRunner, ctx.Tools);
        }

        /// <summary>
        /// Dedicated alias for working with `app.info` files in a project
        /// </summary>
        /// <param name="ctx">The Cake context</param>
        /// <param name="projectFile">Path to the project file of the app you want to work with</param>
        /// <returns>A manager to control app.info generation</returns>
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
            if (ctx.Environment.IsUnix())
                throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            var infoPath = projectFile.GetDirectory().GetFilePath("app.info");
            return ctx.FileSystem.Exist(infoPath)
                ? new AppInfoManager(AppInfoManager.ReadFromFile(infoPath.MakeAbsolute(ctx.Environment).FullPath))
                : new AppInfoManager(projectFile.MakeAbsolute(ctx.Environment).FullPath);
        }

        /// <summary>
        /// Property alias for working with the individual stages in a publish operation independently.
        /// Used in the "split form" of publishing
        /// </summary>
        /// <param name="ctx">The Cake context</param>
        /// <returns>A handler for running isolated individual publish steps</returns>
        [CakePropertyAlias]
        [CakeNamespaceImport("ClickTwice.Templating")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Resources")]
        [CakeNamespaceImport("ClickTwice.Publisher.Core.Handlers")]
        [CakeNamespaceImport("ClickTwice.Handlers.AppDetailsPage")]
        public static ClickTwiceRunner ClickTwice(this ICakeContext ctx)
        {
            if (ctx.Environment.IsUnix())
                throw new PlatformNotSupportedException("ClickTwice is currently only supported on the Windows platform");
            return new ClickTwiceRunner(ctx.Log, ctx.Environment, ctx.FileSystem, ctx.ProcessRunner, ctx.Tools);
        }
    }
}