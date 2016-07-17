using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClickTwice.Publisher.Core.Handlers;

namespace Cake.ClickTwice
{
    public class PublishException : Exception
    {
        public PublishException(string message) : base(message)
        {
        }

        public PublishException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PublishException(IEnumerable<HandlerResponse> responses, string message ) : this(message)
        {
            Errors = responses;
        }

        public PublishException(IEnumerable<HandlerResponse> responses) : this(responses, string.Join(Environment.NewLine, responses.Select(r => r.ResultMessage)))
        {
            
        }

        protected PublishException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<HandlerResponse> Errors { get; set; }
    }
}