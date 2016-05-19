using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace ClickTwice.Handlers.LaunchPage
{
    public class EmbeddedTemplateManager : ITemplateManager
    {
        public EmbeddedTemplateManager(ResourceManager resources)
        {
            Resources = resources;
        }

        private ResourceManager Resources { get; set; }

        /// <exception cref="MissingManifestResourceException">No usable set of resources has been found, and there are no resources for the default culture. For information about how to handle this exception, see the "Handling MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions" section in the <see cref="T:System.Resources.ResourceManager" /> class topic. </exception>
        /// <exception cref="MissingSatelliteAssemblyException">The default culture's resources reside in a satellite assembly that could not be found. For information about how to handle this exception, see the "Handling MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions" section in the <see cref="T:System.Resources.ResourceManager" /> class topic.</exception>
        public ITemplateSource Resolve(ITemplateKey key)
        {
            return new EmbeddedTemplateSource(Resources.GetString(key.Name));
        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new EmbeddedTemplateKey(name, ResolveType.Global, context);
        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException();
        }
    }

    internal class EmbeddedTemplateKey : BaseTemplateKey
    {
        public EmbeddedTemplateKey(string resourceName, ResolveType resolveType, ITemplateKey context)
            : base(resourceName, resolveType, context)
        {
            ResourceName = resourceName;
        }

        private string ResourceName { get; set; }

        public override string GetUniqueKeyString()
        {
            return this.ResourceName;
        }
    }

    internal class EmbeddedTemplateSource : ITemplateSource
    {
        public EmbeddedTemplateSource(string resourceContent)
        {
            this.Content = resourceContent;
        }

        private string Content { get; }

        public TextReader GetTemplateReader() => new StringReader(Content);

        public string TemplateFile => null;

        public string Template => Content;
    }
}