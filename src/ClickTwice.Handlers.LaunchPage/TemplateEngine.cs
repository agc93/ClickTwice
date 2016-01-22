using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace ClickTwice.Handlers.LaunchPage
{
    class TemplateEngine
    {
        private IRazorEngineService Engine => RazorEngine.Engine.Razor;

        public TemplateEngine(AppManifest manifest)
        {
            this.Manifest = manifest;
        }

        private AppManifest Manifest { get; set; }

        public TemplateEngine(ExtendedAppInfo appInfo, AppManifest manifest) : this(manifest)
        {
            this.AppInfo = appInfo;
        }

        private ExtendedAppInfo AppInfo { get; set; }

        internal string BuildPage()
        {
        }

        internal FileInfo WritePage(string page, string path)
        {
            if (path.EndsWith(".html") || path.EndsWith(".htm"))
            {
                File.WriteAllText(path, page, Encoding.UTF8);
                return new FileInfo(path);
            }
            else
            {
                if (!new DirectoryInfo(path).Exists)
                    throw new DirectoryNotFoundException($"Must specify a valid directory for output");
                File.WriteAllText(Path.Combine(path, "index.html"), page, Encoding.UTF8);
                return new FileInfo(Path.Combine(path, "index.html"));
            }
        }
    }
}