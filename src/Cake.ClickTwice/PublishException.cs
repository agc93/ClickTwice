using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClickTwice.Publisher.Core.Handlers;

namespace Cake.ClickTwice
{
    /// <summary>
    /// Thrown when an error occurs during publishing
    /// </summary>
    public class PublishException : Exception
    {
        /// <summary>
        /// Create a new <see cref="PublishException"/> exception with the given text
        /// </summary>
        /// <param name="message">The message to return to the user</param>
        public PublishException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new <see cref="PublishException"/> exception with the given text and inner exception
        /// </summary>
        /// <param name="message">The message to return to the user</param>
        /// <param name="innerException">The inner exception</param>
        public PublishException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Create a new <see cref="PublishException"/> exception with the given text and handler responses
        /// </summary>
        /// <param name="responses">The responses from the handlers collection that threw the exception</param>
        /// <param name="message">The message to return to the user</param>
        public PublishException(IEnumerable<HandlerResponse> responses, string message) : this(message)
        {
            Errors = responses;
        }

        /// <summary>
        /// Create a new <see cref="PublishException"/> exception with the given handler responses
        /// </summary>
        /// <param name="responses">The responses from the handlers collection that threw the exception</param>
        public PublishException(IEnumerable<HandlerResponse> responses)
            : this(responses, string.Join(Environment.NewLine, responses.Select(r => r.ResultMessage)))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PublishException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Collection of responses from the handlers that threw the exception
        /// </summary>
        public IEnumerable<HandlerResponse> Errors { get; set; } = new List<HandlerResponse>();
    }
}