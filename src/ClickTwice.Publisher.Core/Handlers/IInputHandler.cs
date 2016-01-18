namespace ClickTwice.Publisher.Core.Handlers
{
    public interface IInputHandler : IHandler
    {
        HandlerResponse Process(string inputPath);
    }

    public interface IHandler
    {
        string Name { get; }
    }

    public class MockInputHandler : IInputHandler
    {
        public HandlerResponse Process(string inputPath)
        {
            return new HandlerResponse(this, true, "Processed input");
        }

        public string Name => "Mock Input";
    }
}