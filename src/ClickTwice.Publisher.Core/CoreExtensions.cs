using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ClickTwice.Publisher.Core.Handlers;

namespace ClickTwice.Publisher.Core
{
    public static class CoreExtensions
    {
        [DebuggerStepThrough]
        public static void Add<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }

        [DebuggerStepThrough]
        public static void Rename(this FileInfo file, string newName)
        {
            var singleOccurrence = file.FullName.IndexOf(file.Name) == file.FullName.LastIndexOf(file.Name);
            if (singleOccurrence)
            {
                file.MoveTo(file.FullName.Replace(file.Name, newName));
                return;
            }
            var firstPart = file.FullName.Substring(0, file.FullName.LastIndexOf(file.Name));
            var fragment = file.FullName.Substring(file.FullName.LastIndexOf(file.Name));
            fragment = fragment.Replace(file.Name, newName);
            file.MoveTo(firstPart + fragment);
        }


        /// <exception cref="DirectoryNotFoundException">Source directory does not exist.</exception>
        [DebuggerStepThrough]
        public static void Copy(this DirectoryInfo sourceDir, string destDirPath, bool copySubDirs, bool overwrite = true)
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
                file.CopyTo(temppath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirPath, subdir.Name);
                subdir.Copy(temppath, true, overwrite);
            }
        }

        [DebuggerStepThrough]
        public static string Property(this List<string> propList, string property)
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
        public static List<HandlerResponse> ProcessHandlers(this IEnumerable<IInputHandler> handlers, string directoryPath,
            Action<string> logCallback)
        {
            Func<KeyValuePair<IHandler, HandlerResponse>, KeyValuePair<IHandler, HandlerResponse>> selector = r =>
            {
                r.Value.Result = r.Value.Result == HandlerResult.NotRun ? HandlerResult.OK : r.Value.Result;
                logCallback?.Invoke($"Handler {r.Key.Name} returned {r.Value.Result}: {r.Value.ResultMessage}");
                return r;
            };
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return null;
            }
            var handlerList = handlers as IList<IInputHandler> ?? handlers.ToList();
            var responses = handlerList
                .ToDictionary(k => k as IHandler, v => v.Process(directoryPath))
                .Select(selector).Select(d => d.Value);
            return responses.ToList();
        }

        public static List<HandlerResponse> ProcessHandlers(this IEnumerable<IOutputHandler> handlers, string directoryPath,
            Action<string> logCallback)
        {
            Func<KeyValuePair<IHandler, HandlerResponse>, KeyValuePair<IHandler, HandlerResponse>> selector = r =>
            {
                r.Value.Result = r.Value.Result == HandlerResult.NotRun ? HandlerResult.OK : r.Value.Result;
                logCallback?.Invoke($"Handler {r.Key.Name} returned {r.Value.Result}: {r.Value.ResultMessage}");
                return r;
            };
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return null;
            }
            var handlerList = handlers as IList<IOutputHandler> ?? handlers.ToList();
            var responses = handlerList
                .ToDictionary(k => k as IHandler, v => v.Process(directoryPath))
                .Select(selector).Select(d => d.Value);
            return responses.ToList();
        }

        public static List<string> ToTargets(this PublishBehaviour behaviour)
        {
            var targets = new List<string>() { "PrepareForBuild" };
            switch (behaviour)
            {
                case PublishBehaviour.None:
                    targets.Add("Build", "Publish");
                    break;
                case PublishBehaviour.CleanFirst:
                    targets.Add("Clean", "Build", "Publish");
                    break;
                case PublishBehaviour.DoNotBuild:
                    targets.Add("Publish");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }
            return targets;
        }

        public static string ToVersionString(this string version)
        {
            // remember %2a == *
            var segments = version.Split('.').ToList();
            while (segments.Count < 4)
            {
                segments.Add(".%2a");
            }
            return string.Join(".", segments);
        }
    }
}
