using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace ClickTwice.Handlers.AppDetailsPage
{
    class PackageTemplateManager : ITemplateManager
    {
        public PackageTemplateManager(DirectoryInfo templateDirectory)
        {
            PackageDirectory = templateDirectory;
        }

        private DirectoryInfo PackageDirectory { get; set; }

        public ITemplateSource Resolve(ITemplateKey key)
        {
            return new PackageTemplateSource(new FileInfo(key.GetUniqueKeyString()));
        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            var files = PackageDirectory.GetFiles("*.cshtml");
            var tree = PackageDirectory.GetDirectories().Select(d => d.GetFiles("*.cshtml"));
            return new PackageKey(name, resolveType, context);
        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException();
        }
    }

    internal class PackageTemplateSource  : ITemplateSource
    {
        public PackageTemplateSource(FileInfo file)
        {
            File = file;
        }

        private FileInfo File { get; set; }

        public TextReader GetTemplateReader()
        {
            return File.OpenText();
        }

        public string TemplateFile => File.FullName;
        public string Template => System.IO.File.ReadAllText(File.FullName);
    }

    internal class PackageKey :  BaseTemplateKey
    {
        private DirectoryInfo Directory { get; set; }
        private FileInfo File { get; set; }
        private string Id { get; set; }

        public PackageKey(string name, ResolveType resolveType, ITemplateKey context) : base(name, resolveType, context)
        {
            File = new FileInfo(name);
            Directory = File.Directory;
        }

        public override string GetUniqueKeyString()
        {
            return File.FullName;
        }
    }
}
