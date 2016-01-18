namespace ClickTwice.Publisher.Core.Handlers
{
    public interface IOutputHandler : IHandler
    {
        HandlerResponse Process(string outputPath);
    }
}
