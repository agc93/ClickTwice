using System.Collections.Generic;
using Cake.Common.Tools.MSBuild;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace Cake.ClickTwice
{
    public static class ClickTwiceManagerExtensions
    {
        public static ClickTwiceManager SetBuildPlatform(this ClickTwiceManager manager, MSBuildPlatform platform)
        {
            manager.Platform = platform == MSBuildPlatform.Automatic ? "AnyCPU" : platform.ToString();
            return manager;
        }

        public static ClickTwiceManager SetConfiguration(this ClickTwiceManager manager, string configuration)
        {
            manager.Configuration = configuration;
            return manager;
        }

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

        public static ClickTwiceManager WithHandlers(this ClickTwiceManager manager, IEnumerable<IHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                manager.WithHandler(handler);
            }
            return manager;
        }

        public static ClickTwiceManager LogTo(this ClickTwiceManager manager, IPublishLogger logger)
        {
            manager.Loggers.Add(logger);
            return manager;
        }

        public static ClickTwiceManager CleanAfterBuild(this ClickTwiceManager manager)
        {
            manager.CleanOutput = true;
            return manager;
        }

        public static ClickTwiceManager ThrowOnHandlerFailure(this ClickTwiceManager manager)
        {
            manager.ThrowOnHandlerFailure = true;
            return manager;
        }
    }
}