#
# Script.ps1
#
#Param (
#	[string]$ProjectDir
#)

#$ProjectFile = Get-ChildItem -File "*.csproj" | Select-Object -Property FullName
$ProjectFile = "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Templates.SolidState\ClickTwice.Templates.SolidState.csproj"
$ProjectXml = [xml](Get-Content $ProjectFile)
$ns = new-object Xml.XmlNamespaceManager $ProjectXml.NameTable
$ns.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
$ContentFiles = $ProjectXml.SelectNodes("//msb:Project/msb:ItemGroup/msb:Content", $ns)
$ContentFiles | Select-Object -Property Include | select {$_.Attributes.ItemOf["Include"] -ne ""}
$AppVersion = $ProjectXml.SelectSingleNode("//msb:Project/msb:PropertyGroup/msb:ApplicationVersion", $ns)
$AppVersion.InnerText = $newExeVersion