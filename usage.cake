#r "src/packages/Newtonsoft.Json.8.0.2/lib/net45/Newtonsoft.Json.dll"
#r "artifacts/build/Cake.ClickTwice/Cake.ClickTwice.dll"
#r "artifacts/build/ClickTwice.Publisher.Core/ClickTwice.Publisher.Core.dll"
#r "artifacts/ClickTwice.Handlers.LaunchPage.dll"
//#r "artifacts/build/ClickTwice.Handlers.LaunchPage/ClickTwice.Handlers.LaunchPage.dll"
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Resources;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Handlers.LaunchPage;

Task("Publish")
    .Does(() =>
{
    CreateDirectory("./artifacts/publish");
    #break
    ClickTwice(File(@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj"))
                .SetConfiguration("Debug")
                .ThrowOnHandlerFailure()
                .ForceRebuild()
                .WithHandler(new AppInfoHandler(new AppInfoManager()))
                .WithHandler(new LaunchPageHandler())
                .PublishTo("./artifacts/publish/");
});

RunTarget("Publish");