namespace ClickTwice.Publisher.Core.Handlers
{
    public class HandlerResponse
    {
        public HandlerResult Result { get; set; }
        public IHandler Handler { get; set; }
        public string ResultMessage { get; private set; } = string.Empty;

        private HandlerResponse(IHandler handler)
        {
            Handler = handler;
        }
        public HandlerResponse(IHandler handler, bool succeeded) : this(handler)
        {
            Result = succeeded ? HandlerResult.OK : HandlerResult.Error;
        }

        public HandlerResponse(IHandler handler, bool succeeded, string message) : this(handler, succeeded)
        {
            ResultMessage = message;
        }

        public HandlerResponse(IHandler handler, HandlerResult result, string message) : this(handler)
        {
            Result = result;
            ResultMessage = message;
        }
    }

    public enum HandlerResult
    {
        NotRun,
        OK,
        Error
    }
}