using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Common.Tools.MSBuild;
using ClickTwice.Publisher.Core;

namespace Cake.ClickTwice
{
    internal static class CakePublishExtensions
    {
        internal static void AddTargets(this MSBuildSettings settings, PublishBehaviour behaviour)
        {
            foreach (var target in GetTargets(behaviour))
            {
                settings.WithTarget(target);
            }
        }

        private static List<string> GetTargets(PublishBehaviour behaviour)
        {
            var targets = new List<string> {"PrepareForBuild"};
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

        internal static void AddProperties(this MSBuildSettings settings, params Dictionary<string, string>[] dicts)
        {
            var props = dicts.First();
            foreach (var dict in dicts.Skip(1))
            {
                props = props.Concat(dict.Where(p => !props.ContainsKey(p.Key)))
                    .ToDictionary(k => k.Key, v => v.Value);
            }
            foreach (var prop in props)
            {
                settings.WithProperty(prop.Key, prop.Value);
            }
        }

        internal static string GetDirectoryPath(this Cake.Core.IO.FilePath path, Cake.Core.ICakeEnvironment environment)
        {
            return path.GetDirectory().MakeAbsolute(environment).FullPath;
        }
    }
}