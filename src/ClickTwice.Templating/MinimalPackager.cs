using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClickTwice.Publisher.Core;

namespace ClickTwice.Templating
{
    internal class MinimalPackager : IPackager
    {
        public List<string> GetContentFiles(string rootDirectory)
        {
            var files = new DirectoryInfo(rootDirectory).EnumerateFilesForExtensions(false, ".nupkg", ".nuspec", ".config");
            return
                files.Select(f => f.FullName)
                    .Select(n => n.Replace(rootDirectory, string.Empty).Trim().TrimStart('\\'))
                    .ToList();
            //return
            //    new DirectoryInfo(rootDirectory).GetFilesExceptExtensions(".nupkg", ".nuspec", ".dll", ".config")
            //        .Select(f => f.FullName.Replace(rootDirectory, string.Empty))
            //        .ToList();
        }
    }
}