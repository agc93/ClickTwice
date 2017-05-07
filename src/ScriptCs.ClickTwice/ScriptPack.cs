using System;
using System.Reflection;
using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
    public class ScriptPack : ScriptPack<ClickTwicePack>
    {
        public override void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("ScriptCs.ClickTwice");
            
            //session.AddMSBuildReferences();
            session.AddClickTwiceReferences();
            base.Initialize(session);
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Microsoft.Build"))
            {
                Console.WriteLine($"== {args.Name}");
            }
            return null;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.FullName.StartsWith("Microsoft.Build"))
            {
                Console.WriteLine($"++ {args.LoadedAssembly.FullName}");
            }
        }

        public override IScriptPackContext GetContext()
        {
            return new ClickTwicePack();
        }
    }
}