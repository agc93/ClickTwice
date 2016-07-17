namespace ClickTwice.Publisher.Core.Handlers
{
    internal class MockInputHandler : IInputHandler
    {
        public HandlerResponse Process(string inputPath)
        {
            return new HandlerResponse(this, true, "Processed input");
        }

        public string Name => "Mock Input";
    }
}