namespace ClickTwice.Publisher.Core.Handlers
{
    public class MockOutputHandler : IOutputHandler
    {
        public HandlerResponse Process(string outputPath)
        {
            return new HandlerResponse(this, true, "Finished processing");
        }

        public string Name => "Mock Output";
    }
}