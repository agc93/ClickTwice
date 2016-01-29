using System;
using System.IO;
using System.Linq;
using RazorEngine.Templating;

namespace ClickTwice.Handlers.AppDetailsPage.Templating
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
            var files = PackageDirectory.EnumerateFiles("*.cshtml", SearchOption.AllDirectories);
            var template = files.FirstOrDefault(f => f.Name.Replace(f.Extension, string.Empty) == name);
            if (template == null) throw new FileNotFoundException("Could not find requested template", name);
            return new PackageKey(template.FullName, resolveType, context);
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
