using System.Collections.Generic;
using ClickTwice.Publisher.Core.Loggers;

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

    public interface IBuildConfigurator : IHandler
    {
        Dictionary<string, string> ProcessConfiguration(Dictionary<string, string> configuration);
        //List<IPublishLogger> AddLoggers(); //deprecating as it causes problems
        List<string> ProcessTargets(List<string> targets);
    }

    public class BuildConfigurator : IBuildConfigurator
    {
        public virtual string Name => "Base class for build configurators";
        public virtual Dictionary<string, string> ProcessConfiguration(Dictionary<string, string> configuration)
        {
            return null;
        }

        public virtual List<string> ProcessTargets(List<string> targets)
        {
            return new List<string>();
        }
    }

    //public class LoggingConfigurator : BuildConfigurator
    //{
    //    public LoggingConfigurator(List<IPublishLogger> loggers)
    //    {
    //        this.Loggers = loggers;
    //    }

    //    public LoggingConfigurator(IPublishLogger logger) : this(new List<IPublishLogger> { logger})
    //    {
            
    //    }

    //    private List<IPublishLogger> Loggers { get; set; }

    //    public override string Name => "Logging configurator";

    //    public override List<IPublishLogger> AddLoggers()
    //    {
    //        return Loggers;
    //    }
    //}
}