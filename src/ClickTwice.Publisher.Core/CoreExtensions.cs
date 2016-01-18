using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core
{
    internal static class CoreExtensions
    {
        internal static void Add<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }

        /// <exception cref="DirectoryNotFoundException">Source directory does not exist.</exception>
        internal static void Copy(this DirectoryInfo sourceDir, string destDirPath, bool copySubDirs)
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
    }
}
