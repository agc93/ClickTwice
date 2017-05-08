using ClickTwice.Publisher.MSBuild;
using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
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
            session.ImportNamespace("ClickTwice.Publisher.Core.Loggers");
            session.ImportNamespace("ClickTwice.Templating");
            //session.ImportNamespace("ClickTwice.Handlers.AppDetailsPage");
            //TODO reenable
        }
    }
}