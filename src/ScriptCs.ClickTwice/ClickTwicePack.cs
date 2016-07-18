using ScriptCs.Contracts;

namespace ScriptCs.ClickTwice
{
    public class ClickTwicePack : ScriptPack<ClickTwiceContext>
    {

        public override void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("ScriptCs.ClickTwice");
            session.AddReference("ClickTwice.Publisher.Core");
            base.Initialize(session);
        }

        public override IScriptPackContext GetContext()
        {
            return new ClickTwiceContext();
        }
    }
}