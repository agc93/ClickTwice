using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Templating;

namespace Cake.ClickTwice
{
    public static class TemplatePackageSettingsExtensions
    {
        public static TemplatePackageSettings AddAuthor(this TemplatePackageSettings settings, params string[] authors)
        {
            settings.Authors.AddRange(authors);
            return settings;
        }

        public static TemplatePackageSettings UseDescription(this TemplatePackageSettings settings, string description)
        {
            settings.Description = description;
            return settings;
        }

        public static TemplatePackageSettings UsePackageId(this TemplatePackageSettings settings, string id)
        {
            settings.Id = id;
            return settings;
        }

        public static TemplatePackageSettings UseVersion(this TemplatePackageSettings settings, string version)
        {
            settings.Version = version;
            return settings;
        }
    }
}
