using ClickTwice.Publisher.Core;

namespace ScriptCs.ClickTwice
{
    public class Publisher
    {
        internal Publisher(IPublishManager mgr)
        {
            Manager = mgr;
        }

        public void To(string outputPath)
        {
            Manager.PublishApp(outputPath);
        }

        private IPublishManager Manager { get; set; }
    }
}