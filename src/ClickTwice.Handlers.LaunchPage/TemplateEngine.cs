using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core.Manifests;
using ClickTwice.Publisher.Core.Resources;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace ClickTwice.Handlers.LaunchPage
{
    class TemplateEngine
    {
        private IRazorEngineService Engine { get; }

        public TemplateEngine(AppManifest manifest)
        {
            this.Manifest = manifest;
            TemplateSource = new EmbeddedTemplateManager(Templates.ResourceManager);
            var config = new TemplateServiceConfiguration()
            {
                TemplateManager = TemplateSource,
            };
            Engine = RazorEngineService.Create(config);
        }

        private EmbeddedTemplateManager TemplateSource { get; set; }

        private AppManifest Manifest { get; set; }

        public TemplateEngine(ExtendedAppInfo appInfo, AppManifest manifest) : this(manifest)
        {
            this.AppInfo = appInfo;
        }

        private ExtendedAppInfo AppInfo { get; set; }

        internal string BuildPage(string templateName)
        {
            var templateKey = TemplateSource.GetKey(templateName, ResolveType.Global, null);
            var result = Engine.RunCompile(templateKey, typeof (LaunchPageModel), new LaunchPageModel(Manifest, AppInfo));
            return result;
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