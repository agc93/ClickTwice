# ClickTwice page templates

## The model object

The ClickTwice Core library includes a simple model class called the `LaunchPageModel`, with a basic structure as below

```csharp
public class LaunchPageModel {
    // trimmed
    public AppManifest Manifest { get; set; }
    public ExtendedAppInfo AppInfo { get; set; }
    public string ContentDirectory { get; set; }
    public string Launcher { get; set; }
    public string Installer { get; set; }
}
```

Now this model is never directly used by the core but can instead be consumed by handlers for generating practically anything. Out of the box, the powerful `AppDetailsPageHandler` uses this model to generate fast and simple app pages (perfect for intranets or GitHub Pages!), but you can use the same basic model for almost anything.

## Creating app details templates

The `AppDetailsPageHandler` included with most ClickTwice packages is powered by Razor and NuGet, the same tools you use to build pages every day! That means that you can point it any NuGet package containing `.cshtml` files to quickly generate whole websites at a time. You can find more details on the `AppDetailsPageHandler` in it's [own documentation](/doc/appdetailspage.html).

A ClickTwice template package is dead simple to create. All you need is a Razor view file (`.cshtml` extension) that can take a `LaunchPageModel` object as it's model and you're ready to go. To make it easier, ClickTwice ships with template packaging and publishing built-in, so you don't even need to switch tools!

For Cake:
```csharp
Task("Template-Publishing")
.Does(() => {
	ClickTwice.PublishTemplate("./path/to/template/dir/", s => 
		s.AddAuthor("Alistair Chapman")
			.UsePackageId("TemplatePackage")
			.UseVersion("0.0.1")
			.UseDescription("Optional description"))
	.ToPackageFile("artifacts/publish.nupkg")
	.ToGallery(galleryUri: "http://nuget.org/api/v2");
});
```

or for ScriptCs:

```csharp
PublishTemplate("C:/path/to/template/directory")
    .SetMetadata("Package.Id", "0.0.1", "Author Name")
    .ToPackageFile("artifacts/publish.nupkg")
    .ToGallery(galleryUri: "http://nuget.org/api/v2");
```

Then you can consume your new template in the `AppDetailsPageHandler`:

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
