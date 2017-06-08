using System.Collections.Generic;
using Cake.Common.Tools.MSBuild;
using System.Linq;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace Cake.ClickTwice
{
    /// <summary>
    /// Extension methods for the <see cref="ClickTwiceManager"/>
    /// </summary>
    public static class ClickTwiceManagerExtensions
    {
        /// <summary>
        /// Sets the build platform to use when invoking MSBuild
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="platform">The <see cref="MSBuildPlatform"/> platform to build for</param>
        /// <returns>The updated manager</returns>
        public static ClickTwiceManager SetBuildPlatform(this ClickTwiceManager manager, MSBuildPlatform platform)
        {
            manager.Platform = platform == MSBuildPlatform.Automatic ? "AnyCPU" : platform.ToString();
            return manager;
        }

        /// <summary>
        /// Sets the build configuration to use when invoking MSBuild
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="configuration">The configuration to build for</param>
        /// <returns>The updated manager</returns>
        public static ClickTwiceManager SetConfiguration(this ClickTwiceManager manager, string configuration)
        {
            manager.Configuration = configuration;
            return manager;
        }

        /// <summary>
        /// Adds an input or output handler to the build pipeline
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="handler">The handler (<see cref="IInputHandler"/> or <see cref="IOutputHandler"/>) to use</param>
        /// <returns>The updated manager</returns>
        /// <remarks>If a given object implements both <see cref="IInputHandler"/> and <see cref="IOutputHandler"/> it will be added to both stages of the build pipeline</remarks>
        public static ClickTwiceManager WithHandler(this ClickTwiceManager manager, IHandler handler)
        {
            var output = handler as IOutputHandler;
            var input = handler as IInputHandler;
            if (output != null)
            {
                manager.OutputHandlers.Add(output);
            }
            if (input != null)
            {
                manager.InputHandlers.Add(input);
            }
            return manager;
        }

        /// <summary>
        /// Adds multiple input or output handlers to the build pipeline
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="handlers">A collection of <see cref="IHandler"/>-based handlers to use</param>
        /// <returns>The updated manager</returns>
        /// <remarks>If a given object implements both <see cref="IInputHandler"/> and <see cref="IOutputHandler"/> it will be added to both stages of the build pipeline</remarks>
        public static ClickTwiceManager WithHandlers(this ClickTwiceManager manager, IEnumerable<IHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                manager.WithHandler(handler);
            }
            return manager;
        }

        /// <summary>
        /// Adds a new <see cref="IPublishLogger"/> to the Loggers collection
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="logger">A <see cref="IPublishLogger"/> to log messages to</param>
        /// <returns>The updated manager</returns>
        public static ClickTwiceManager LogTo(this ClickTwiceManager manager, IPublishLogger logger)
        {
            manager.Loggers.Add(logger);
            return manager;
        }

        /// <summary>
        /// Enables cleaning the output directory after a complete build
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <returns>The updated manager</returns>
        public static ClickTwiceManager CleanAfterBuild(this ClickTwiceManager manager)
        {
            manager.CleanOutput = true;
            return manager;
        }

        /// <summary>
        /// Enables aborting the build if an input handlers fails with an error
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <returns>The updated manager</returns>
        /// <remarks>Handlers returning <see cref="HandlerResult.NotRun"/> will be ignored.</remarks>
        public static ClickTwiceManager ThrowOnHandlerFailure(this ClickTwiceManager manager)
        {
            manager.ErrorAction =
                resp => { throw new PublishException(resp.Where(r => r.Result == HandlerResult.Error)); };
            return manager;
        }

        /// <summary>
        /// Enables forcing a rebuild of the application, even if a build is up-to-date
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <returns>The updated manager</returns>
        public static ClickTwiceManager ForceRebuild(this ClickTwiceManager manager)
        {
            manager.Behaviour = PublishBehaviour.CleanFirst;
            return manager;
        }

        /// <summary>
        /// Publishes the application without building it first.
        /// </summary>
        /// <remarks>This essentially means that we only pass the <c>Publish</c> target to MSBuild.</remarks>
        /// <param name="manager">The manager</param>
        /// <returns>The updated manager.</returns>
        public static ClickTwiceManager DoNotBuild(this ClickTwiceManager manager)
        {
            manager.Behaviour = PublishBehaviour.DoNotBuild;
            return manager;
        }

        /// <summary>
        /// Manually sets the publish version to the provided value
        /// </summary>
        /// <param name="manager">The manager</param>
        /// <param name="s">The version string to use</param>
        /// <returns></returns>
        public static ClickTwiceManager WithVersion(this ClickTwiceManager manager, string s)
        {
            manager.PublishVersion = s;
            return manager;
        }

        //public static ClickTwiceManager UseBuildAction(this ClickTwiceManager manager,
        //    Action<CakePublishManager> buildAction)
        //{
        //    manager.BuildAction = buildAction;
        //    return manager;
        //}
    }
}