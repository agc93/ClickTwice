using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;

namespace Cake.ClickTwice
{
    [CakeAliasCategory("ClickOnce")]
    public static class ClickTwiceAliases
    {
        [CakeMethodAlias]
        public static ClickTwiceManager ClickTwice(this ICakeContext ctx, FilePath projectFile)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            return new ClickTwiceManager(projectFile, ctx);
        }
    }
}
