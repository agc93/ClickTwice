using System.Collections.Generic;

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

    public interface IBuildConfigurator : IHandler
    {
        Dictionary<string, string> ProcessConfiguration(Dictionary<string, string> configuration);
        List<string> ProcessTargets(List<string> targets);
    }

    public interface IOutputHandler : IHandler
    {
        HandlerResponse Process(string publishPath);
    }
}