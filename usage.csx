#r "ScriptCs.ClickTwice.dll"
using ScriptCs.ClickTwice;
var pack = Require<ClickTwiceContext>();

//pack.Configure(s => s.WithLogger(new ConsoleLogger()));
//pack.PublishApp(@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj").To("./artifacts/publish");