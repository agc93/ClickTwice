using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClickTwice.Publisher.Core.Exceptions
{
    [Serializable]
    internal class BuildFailedException : Exception
    {
        public IEnumerable<Exception> TargetExceptions { get; }
        public Exception Exception { get; }

        public BuildFailedException()
        {
        }

        public BuildFailedException(string message) : base(message)
        {
        }

        public BuildFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BuildFailedException(Exception exception, IEnumerable<Exception> innerExceptions)
        {
            this.Exception = exception;
            this.TargetExceptions = innerExceptions;
        }

        protected BuildFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}