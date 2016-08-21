using System;
using System.Reflection;
using ClickTwice.Publisher.MSBuild;
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

    internal static class SessionExtensions
    {
        internal static void AddMSBuildReferences(this IScriptPackSession session)
        {
            session.AddReference(MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build"));
            session.AddReference(MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Framework"));
            session.AddReference(MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Tasks") ??
                                 MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Tasks.Core") ??
                                 MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Tasks.v4.0"));
            session.AddReference(MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Utilities.Core") ??
                                 MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Utilities") ??
                                 MSBuildResolver.GetBuildAssemblyPath("Microsoft.Build.Utilities.v4.0"));

            session.ImportNamespace("Microsoft.Build");
            session.ImportNamespace("Microsoft.Build.Framework");
            session.ImportNamespace("Microsoft.Build.Evaluation");
            session.ImportNamespace("Microsoft.Build.Execution");
        }

        internal static void AddClickTwiceReferences(this IScriptPackSession session)
        {
            session.AddReference("ClickTwice.Publisher.Core");
            //session.AddReference("ClickTwice.Handlers.AppDetailsPage");
            session.AddReference("ClickTwice.Templating");

            session.ImportNamespace("ClickTwice.Publisher.Core");
            session.ImportNamespace("ClickTwice.Publisher.Core.Resources");
            session.ImportNamespace("ClickTwice.Publisher.Core.Handlers");
            session.ImportNamespace("ClickTwice.Templating");
            //session.ImportNamespace("ClickTwice.Handlers.AppDetailsPage");
            //TODO reenable
        }
    }
}