using System;
using System.Linq;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;

namespace ScriptCs.ClickTwice
{
    public class Publisher
    {

        internal Publisher(IPublishManager mgr)
        {
            Manager = mgr;
        }

        internal IScriptHost Host { private get; set; }

        public void To(string outputPath)
        {
            try
            {
                Host?.Log($"Publishing to {outputPath}");
                Manager.PublishApp(outputPath);
                Host?.Log("Publish operation completed!");
            }
            catch (HandlerProcessingException ex)
            {
                Host?.Error(string.Join(Environment.NewLine, ex.HandlerResponses.Where(r => r.Result == HandlerResult.Error).Select(r => $"{r.Handler.Name} - {r.ResultMessage}")));
            }
        }

        private IPublishManager Manager { get; set; }
    }
}