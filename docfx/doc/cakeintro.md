# Cake publisher

> Publish ClickOnce apps from your Cake scripts the easy way.

The ClickTwice host for Cake is a standard Cake addin, exposing multiple ways of publishing your app.

## Installation

First, install/include the addin by adding the `#addin` declaration as below.

```
#addin nuget:?package=Cake.ClickTwice

//or to use the latest development release
#addin nuget:?package=Cake.ClickTwice&prerelease
```

The NuGet prerelease packages are automatically built and deployed from the `develop` branch so they can be considered bleeding-edge while the non-prerelease packages will be much more stable.

Versioning is predominantly SemVer-compliant so you can set your version constraints if you're worried about changes.

> [!NOTE]
> These packages include their dependent libraries (including RazorEngine and friends) at this time, as Cake is currently unable to correctly reference NuGet dependencies.

## Usage

To publish your app, just use the `PublishApp` alias, as per below:

```csharp
Task("Publish")
.Does(() =>
{
    PublishApp(yourProjectPathHere).To("./artifacts/publish/");
});
```
That's it! Now that is just a plain MSBuild build and publish, with no additional features. If you want to add handlers, just use `WithHandler` or use `LogTo` to add loggers:

```csharp
Task("Publish")
.Does(() =>
{
    PublishApp(yourProjectPathHere)
        .WithHandler(new InstallPageHandler())
        .LogTo(new FileLogger())
        .To("./artifacts/publish/");
});
```

And there's even more options available from the [`ClickTwiceManager`](/api/Cake.ClickTwice.ClickTwiceManagerExtensions.html) to explore. For example, the following works:

```csharp
Task("Publish")
.Does(() =>
{
    PublishApp(yourProjectPathHere)
        .WithHandler(new InstallPageHandler())
        .LogTo(new FileLogger())
        .SetConfiguration("Debug")
        .ForceRebuild()
        .SetBuildPlatform(MSBuildPlatform.x64)
        .ThrowOnHandlerFailure()
        .WithVersion("0.9.2.1")
        .To("./artifacts/publish/");
});
```

## Split form usage

If you already have customised `MSBuild` alias actions you use, no need to replace them! You can also use the `ClickTwice` property alias to build complex actions:

```csharp
Task("Publish")
.Does(() => 
    var appInfo = new AppInfoHandler();
    ClickTwice.RunInputHandlers(projectPath, appInfo);
    MSBuild(projectPath, settings =>
		settings.SetPlatformTarget(PlatformTarget.MSIL)
            .WithTarget("Publish") //important!
			.SetConfiguration("Debug"));
    //build here
    ClickTwice.GenerateManifest(projectPath).Publish("./path/to/published/files");
    ClickTwice.RunOutputHandlers("./path/to/published/files", appInfo, new InstallPageHandler());
});
```

Note that this is not the recommended method and can be easy to get wrong. Further, reliably determining the "./path/to/published/files" in the above example may not always be trivial. It is usually a folder called "app.publish" in the build output directory. It is strongly recommended to use the `PublishApp` alias instead.