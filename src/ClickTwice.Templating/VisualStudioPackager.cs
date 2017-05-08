using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ClickTwice.Templating
{
    internal class VisualStudioPackager : IPackager
    {
        public List<string> GetContentFiles(string rootDirectory)
        {
            var xml = new XmlDocument();
            xml.Load(new DirectoryInfo(rootDirectory).GetFiles("*.csproj").First().FullName);
            XmlNamespaceManager mgr = new XmlNamespaceManager(xml.NameTable);
            mgr.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
            var nodeList = xml.SelectNodes("//msb:Project/msb:ItemGroup/msb:Content", mgr);
            if (nodeList != null)
            {
                var contentFiles =
                    nodeList.Cast<XmlNode>()
                        .Where(x => !string.IsNullOrWhiteSpace(x.Attributes?["Include"].Value))
                        .Select(x => x.Attributes["Include"].Value);
                return contentFiles.Where(f => !f.EndsWith("config")).ToList();
            }
            return new List<string>();
        }
    }
}