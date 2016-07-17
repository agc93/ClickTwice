//#r "src/packages/Newtonsoft.Json.8.0.2/lib/net45/Newtonsoft.Json.dll"
#r "artifacts/build/Cake.ClickTwice/Cake.ClickTwice.dll"
//#r "artifacts/ClickTwice.dll"
#r "artifacts/lib/ClickTwice.Publisher.Core/ClickTwice.Publisher.Core.dll"

#r "artifacts/lib/ClickTwice.Handlers.AppDetailsPage/ClickTwice.Handlers.AppDetailsPage.dll"

var target = Argument<string>("target", "Publish");
//using ClickTwice.Handlers.AppDetailsPage;
//using ClickTwice.Publisher.Core.Handlers;
//using ClickTwice.Publisher.Core;

var projectPath = File(@"./path/to/the/project.proj");

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
	CleanDirectory("./artifacts/publish");
    CreateDirectory("./artifacts/publish");
});

Task("Publish")
    .Does(() =>
{
    PublishApp(projectPath)
                .SetConfiguration("Debug")
                .ThrowOnHandlerFailure()
                .WithHandler(new AppInfoHandler(new AppInfoManager()))
                .WithHandler(new AppDetailsPageHandler("ClickTwice.Templates.SolidState") {
                    FileNameMap = new Dictionary<string, string> {
                        {"index.html", "details.html"}
                    }
                })
                .WithHandler(new InstallPageHandler(fileName: "index.html", linkText: "Details", linkTarget: "details.html"))
                .PublishTo("./artifacts/publish/");
});

Task("Long-Form")
.Does(() => {
    var handler = new AppInfoHandler(new AppInfoManager());
    ClickTwice.RunInputHandlers(projectPath, handler);
    MSBuild(projectPath, settings =>
		settings.SetPlatformTarget(PlatformTarget.MSIL)
			.WithProperty("PublishDir", Directory("./artifacts/publish").Path.MakeAbsolute(Context.Environment).ToString() + "\\")
			.SetVerbosity(Verbosity.Quiet)
			.WithTarget("Build")
            .WithTarget("Publish")
			.SetConfiguration("Debug"));
    //build here
    ClickTwice.GenerateManifest(projectPath).Publish("./artifacts/publish");
    ClickTwice.RunOutputHandlers("./artifacts/publish", handler, new InstallPageHandler());
});

RunTarget(target);