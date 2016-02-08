using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Framework.XamlTypes;

namespace ClickTwice.Publisher.Core
{
    public static class CoreExtensions
    {
        [DebuggerStepThrough]
        internal static void Add<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }


        /// <exception cref="DirectoryNotFoundException">Source directory does not exist.</exception>
        [DebuggerStepThrough]
        public static void Copy(this DirectoryInfo sourceDir, string destDirPath, bool copySubDirs)
        {
            
            // Get the subdirectories for the specified directory.

            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDir.FullName);
            }

            DirectoryInfo[] dirs = sourceDir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            // Get the files in the directory and copy them to the new location.
            var files = sourceDir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirPath, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirPath, subdir.Name);
                subdir.Copy(temppath, true);
            }
        }

        [DebuggerStepThrough]
        internal static string Property(this List<string> propList, string property)
        {
            var line = propList.FirstOrDefault(s => s.Contains(property));
            return line?.Split('"', '\'')[1] ?? string.Empty;
        }

        [DebuggerStepThrough]
        internal static string FindAttribute(this XElement element, string key)
        {
            return element.Attributes().FirstOrDefault(a => a.Name.LocalName == key)?.Value ?? string.Empty;
        }

        [DebuggerStepThrough]
        internal static string Find(this IEnumerable<string> list, string key)
        {
            return list.FirstOrDefault(_ => _ == key) ?? string.Empty;
        }

        [DebuggerStepThrough]
        internal static void AddIfNotExists(this IEnumerable<string> collection, string entry)
        {
            if (!collection.Contains(entry))
            {
                collection.ToList().Add(entry);
            }
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }

        public static IEnumerable<FileInfo> GetFilesExceptExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            var files = dir.EnumerateFiles();
            return files.Where(f => !extensions.Contains(f.Extension));
        }

        public static IEnumerable<FileInfo> EnumerateFilesForExtensions(this DirectoryInfo dir, bool match,
            params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            var files = dir.EnumerateFiles("*", SearchOption.AllDirectories);
            return (match)
                ? files.Where(f => extensions.Contains(f.Extension))
                : files.Where(f => !extensions.Contains(f.Extension));
        } 
    }
}
