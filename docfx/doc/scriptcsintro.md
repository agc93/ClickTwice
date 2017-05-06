# ScriptCs publish host

> Publish ClickOnce apps from C# scripts the easy way.

The ClickTwice host for ScriptCs is distributed as a script pack, exposing a dead-simple API for publishing your app.

## Installation

First, install the script pack using the command below

```powershell
scriptcs -Install ScriptCs.ClickTwice

# or to use the latest development release
scriptcs -Pre -Install ScriptCs.ClickTwice
```

The NuGet prerelease packages are automatically built and deployed from the `develop` branch so they can be considered bleeding-edge while the non-prerelease packages will be much more stable.

> **NOTE:** The `ScriptCs.ClickTwice` uses NuGet dependencies to bring in some third-party libraries. Check the package details if you are worried about this.

## Usage

To publish your app, just use the `PublishApp` method, as per below:

```csharp
PublishApp(yourProjectPathHere).To("./artifacts/publish/");
```

That's it! Now that is just a plain MSBuild build and publish, with no additional features. If you want to add handlers, you will need to configure your settings object. You can do this by preparing an `Action<ClickTwicePackSettings>` and pass it directly into your `PublishApp` method, or use the `Configure` method

```csharp
Configure(s => s.WithHandler(new AppInfoHandler()).WithLogger(new FileLogger()));
PublishApp(yourProjectPathHere).To("./artifacts/publish");
```

And there's even more options available from the [`ClickTwicePackSettings`](/api/ScriptCs.ClickTwice.ClickTwicePackSettings.html) to explore. For example, the following also works:

```csharp
Configure(s => 
    s.SetConfiguration("Debug")
    .UseIntegratedMsBuild()
    .SetPlatform("AnyCPU")
    .EnableBuildMessages()
    .WithHandler(new AppInfoHandler())
    .WithLogger(new FileLogger())
);
PublishApp(yourProjectPathHere).To("./artifacts/publish/");
```

Plus, since ScriptCs scripts can run any C# code, you can now supercharge your build script with anything you can do with C#!

## C# 6 support

Note that due to an incompatibility between the internal builder that ships with ClickTwice and the way that ScriptCs loads assemblies, we can only build WPF applications using C# 6 features, by using a system-local copy of MSBuild (i.e. 'shelling out').

If you don't have MSBuild installed in your build environment, and don't need C# 6 features, you can build using a special bundled-in MSBuild instance by adding `UseIntegratedMsBuild()` to your configuration.

```csharp
Configure(s => s.UseIntegratedMsBuild());
PublishApp(yourProjectPathHere).To("./artifacts/publish/");
```

```csharp
// alternate short form
PublishApp(yourProjectPathHere, s => s.UseIntegratedMsBuild()).To("./artifacts/publish/");
```

Using this, you don't even need MSBuild installed to build your app!