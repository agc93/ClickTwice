using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Templating;

namespace Cake.ClickTwice
{
    class Samples
    {
        public void Sample()
        {
            var a = new Action<TemplatePackageSettings>(s =>
                s.AddAuthor("Alistair Chapman")
                    .UsePackageId("TemplatePackage")
                    .UseVersion("0.0.1")
                    .UseDescription("Optional description"));
        }
    }
}
