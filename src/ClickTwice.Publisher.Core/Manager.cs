using System.IO;
using System.Linq;

namespace ClickTwice.Publisher.Core
{
    public abstract class Manager
    {
        protected Manager(string projectFilePath)
        {
            this.ProjectFilePath = projectFilePath;
        }

        public string ProjectFilePath { get; protected set; }

        public bool CleanOutputOnCompletion { protected get; set; } = true;

        protected string ReadVersionFromAssemblyInfo()
        {
            var projectFolder = new FileInfo(ProjectFilePath).Directory;
            var infoFilePath = Path.Combine(projectFolder.FullName, "Properties", "AssemblyInfo.cs");
            var props = File.ReadAllLines(infoFilePath).Where(l => l.StartsWith("[assembly: ")).ToList();
            var v = props.Property("Version");
            return v;
        }
    }
}
