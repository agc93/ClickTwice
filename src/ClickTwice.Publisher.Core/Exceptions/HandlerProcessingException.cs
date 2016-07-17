using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClickTwice.Publisher.Core.Handlers;

namespace ClickTwice.Publisher.Core.Exceptions
{
    [Serializable]
    public class HandlerProcessingException : Exception
    {
        private List<IInputHandler> inputHandlers;
        private List<IOutputHandler> outputHandlers;

        public HandlerProcessingException()
        {
        }

        public HandlerProcessingException(string message) : base(message)
        {
        }

        private HandlerProcessingException(List<IInputHandler> inputHandlers)
        {
            this.inputHandlers = inputHandlers;
        }

        public HandlerProcessingException(List<IOutputHandler> outputHandlers)
        {
            this.outputHandlers = outputHandlers;
        }

        public HandlerProcessingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandlerProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HandlerProcessingException(List<IInputHandler> handlers, List<HandlerResponse> results) : this(handlers)
        {
            this.HandlerResponses = results;
        }

        public List<HandlerResponse> HandlerResponses { get; set; }

        public HandlerProcessingException(List<IOutputHandler> handlers, List<HandlerResponse> results) : this(handlers)
        {
            this.HandlerResponses = results;
        }
    }
}