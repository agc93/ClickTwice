using System.Collections.Generic;

namespace ClickTwice.Publisher.Core.Handlers
{
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
}