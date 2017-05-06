# ClickTwice App Page Generator

## Introduction

This powerful handler comes shipped with most ClickTwice packages and provides for dead simple page generation abilities with user-built templates and full support for Razor syntax in templates.

Whereas the `LaunchPageHandler` simply produces a basic one-file template to launch your ClickTwice-published app, using this handler you can generate much more complex sites, built from one or multiple pages. 

## Usage

To use, just add a new instance of `AppDetailsPageHandler` to your handlers using the appropriate method for your host:

```csharp
//for Cake
PublishApp(projectPath)
    .WithHandler(new AppDetailsPageHandler("TemplatePackage")) // your package ID here
    .PublishTo("./artifacts/publish/");
```

```csharp
// for scriptcs
pack.Configure(s => s.WithHandler(new AppDetailsPageHandler("TemplatePackage")));
pack.PublishApp(projectPath).To("./artifacts/publish");
```

Where `"TemplatePackage"` is the NuGet package ID of your chosen template. You can optionally provide a source as well, for use with private NuGet repositories. In addition to specifying a template name as above, you can also provide a `FileInfo` object to use a local `nupkg` file for your template.

> [!WARN]
> The `AppDetailsPageHandler` requires both a standard manifest (`.cltw`) **and** an `app.info` file. The handler will not run without both.

## Template packages

As outlined in the [Templating documentation](/doc/templating.html), ClickTwice templates are simply a NuGet package full of static files and any number of `*.cshtml` Razor view files.

When the handler is run, it will copy all static content into the publish directory (see below), and will then run any `.cshtml` files through the Razor compiler, generating `.html` files in the same relative location.

> [!NOTE]
> If you don't have your own template, the `ClickTwice.Templates.SolidState` package is a good start!

> [!NOTE]
> ClickTwice template packages do not require any special metadata, but are often prefixed with `ClickTwice.Templates`.

### Web files

In order to keep deployment directories clean and simple, ClickTwice does not leave generated views in the deployment root: it puts them in a separate content directory (available as `ContentDirectory` in the `Model`), with the exception of the special view named `index.cshtml` which will be generated in-place.

### File Name Mappings

Using third-party templates is obviously dead simple, but can lead to a problem: incorrect file naming and file naming conflicts. The answer to this in the `AppDetailsPageHandler` is in it's optional `Dictionary<string, string> FileNameMap` property. This dictionary is a simple source-to-target mapping of file names found in the provided template and what file they should be generated as in the destination folder:

```csharp
//for Cake
PublishApp(projectPath)
    .WithHandler(new AppDetailsPageHandler("ClickTwice.Templates.SolidState") {
        FileNameMap = new Dictionary<string, string> {
            {"index.html", "details.html"}
        }
    })
    .PublishTo("./artifacts/publish/");
```

This will generate `index.cshtml` (note the `html` extension) from the *"ClickTwice.Templates.SolidState"* NuGet package in the target folder as `details.html`. 