using ScriptCs.ClickTwice.MSBuild;
using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
    public class ScriptPack : ScriptPack<ClickTwicePack>
    {

        public override void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("ScriptCs.ClickTwice");
            
            session.AddMSBuildReferences();
            session.AddClickTwiceReferences();
            base.Initialize(session);
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
            session.AddReference(MSBuildResolver.GetMSBuildPath("Microsoft.Build"));
            session.AddReference(MSBuildResolver.GetMSBuildPath("Microsoft.Build.Framework"));
            session.AddReference(MSBuildResolver.GetMSBuildPath("Microsoft.Build.Tasks"));
            session.AddReference(MSBuildResolver.GetMSBuildPath("Microsoft.Build.Utilities"));

            session.ImportNamespace("Microsoft.Build");
            session.ImportNamespace("Microsoft.Build.Framework");
            session.ImportNamespace("Microsoft.Build.Evaluation");
            session.ImportNamespace("Microsoft.Build.Execution");
        }

        internal static void AddClickTwiceReferences(this IScriptPackSession session)
        {
            session.AddReference("ClickTwice.Publisher.Core");
            session.AddReference("ClickTwice.Handlers.AppDetailsPage");

            session.ImportNamespace("ClickTwice.Publisher.Core");
            session.ImportNamespace("ClickTwice.Publisher.Core.Resources");
            session.ImportNamespace("ClickTwice.Publisher.Core.Handlers");
            session.ImportNamespace("ClickTwice.Handlers.AppDetailsPage");
        }
    }
}