var pack = Require<ClickTwicePack>();

pack.Configure(s => 
    s
        .SetConfiguration("Debug")
        .WithHandler(new AppInfoHandler(new AppInfoManager()))
        .WithHandler(new InstallPageHandler(fileName: "index.html", linkText: "Details", linkTarget: "details.html"))
);
pack.PublishApp(@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj").To("./artifacts/publish");

PublishTemplate("C:/path/to/template/directory")
    .SetMetadata("Package.Id", "0.0.1", "Author Name")
    .ToPackageFile("artifacts/publish.nupkg")
    .ToGallery(galleryUri: "http://nuget.org/api/v2");