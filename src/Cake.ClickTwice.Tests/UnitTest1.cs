using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using Xunit;
using Xunit.Abstractions;

namespace Cake.ClickTwice.Tests
{
    public class AliasTests
    {
        public AliasTests(ITestOutputHelper helper)
        {
            Output = helper;
        }

        private ITestOutputHelper Output { get; set; }

        [Fact]
        public void CakeAliasTest()
        {
            var fixture = new ClickTwiceFixture(Output);
            var mgr =
                fixture.Run(
                    @"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj");
            mgr.BuildAction = manager =>
            {
                foreach (var logger in manager.Loggers)
                {
                    logger.Log("Building project...");
                }
            };
            mgr.SetConfiguration("Debug")
                .ThrowOnHandlerFailure()
                .ForceRebuild()
                .WithHandler(new AppInfoHandler(new AppInfoManager()))
                .PublishTo(fixture.FileSystem.GetDirectory("./artifacts").Path);

        }
    }
}
