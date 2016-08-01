#r "ScriptCs.ClickTwice.dll"
using ScriptCs.ClickTwice;
using ClickTwice.Publisher.Core.Loggers;
//var t = typeof(ClickTwicePack);
var pack = Require<ClickTwicePack>();

pack.Configure(s => s.WithLogger(new ConsoleLogger()));
pack.PublishApp(@"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj").To("./artifacts/publish");