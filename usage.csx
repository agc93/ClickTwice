var pack = Require<ClickTwicePack>();

pack.Configure(s => 
    s
        .SetConfiguration("Debug")
        .WithHandler(new AppInfoHandler(new AppInfoManager()))
        .WithHandler(new InstallPageHandler(fileName: "index.html", linkText: "Details", linkTarget: "details.html"))
);
pack.PublishApp(@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj").To("./artifacts/publish");