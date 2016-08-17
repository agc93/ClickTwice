using System.Collections.Generic;

namespace ClickTwice.Publisher.Core.Handlers
{
    public abstract class BuildConfigurator : IBuildConfigurator
    {
        public abstract string Name { get; }
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