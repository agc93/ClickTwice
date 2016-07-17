using System;
using System.Collections.Generic;
using System.Security;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;

namespace ClickTwice.Publisher.Core
{
    public interface IPublishManager : IDisposable
    {
        string Configuration { set; }
        string Platform { set; }
        List<IInputHandler> InputHandlers { set; }
        List<IOutputHandler> OutputHandlers { set; }
        List<IBuildConfigurator> BuildConfigurators { set; }
        List<IPublishLogger> Loggers { get; }
        string ProjectFilePath { get; }
        bool CleanOutputOnCompletion { set; }

        /// <exception cref="HandlerProcessingException">Thrown when input or output handlers encounter an exception.</exception>
        /// <exception cref="OperationInProgressException">Thrown when a build or publish operation is already in progress.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid behaviour type provided.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="BuildFailedException">Thrown when the build fails.</exception>
        List<HandlerResponse> PublishApp(string targetPath,
            PublishBehaviour behaviour = PublishBehaviour.CleanFirst);
    }

    public enum OperationType
    {
        Clean,
        Build,
        Publish,
        Deploy
    }

    public enum PublishBehaviour
    {
        None,
        CleanFirst,
        DoNotBuild
    }
}
