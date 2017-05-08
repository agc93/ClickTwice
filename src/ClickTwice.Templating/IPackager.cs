using System.Collections.Generic;

namespace ClickTwice.Templating
{
    internal interface IPackager
    {
        List<string> GetContentFiles(string rootDirectory);
    }
}