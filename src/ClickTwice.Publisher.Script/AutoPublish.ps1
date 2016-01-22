#
# AutoPublish.ps1
#
Param (
	[string]$ProjectFilePath,
	[string]$Configuration,
	[string]$DeployPath
)
Write-Host "ClickTwice Publisher - Script Publish Tool"

$outputPrefix = " "

function CheckMSBuildPath ()
{
    If (-not $env:msbuild) {
        $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.EXE" 
    } Else {
        $msbuild = $env:msbuild
    }
}

function GetProjectDirectory ($projectFile)
{
    return Get-ChildItem $ProjectFilePath | Select-Object -Property DirectoryName
}

function GetApplicationName() {
	[string]$projectName = Get-ChildItem $ProjectFilePath | Select-Object -Property Name
	[string]$extension = $projectName.Substring(0, $projectName.Length) 
}
CheckMSBuildPath
Write-Host $outputPrefix"Cleaning the build directory..."
Invoke-Expression "$msbuild $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:clean /v:quiet /nologo"

Write-Host $outputPrefix"Building Executable application..."
Invoke-Expression "$msbuild $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:build /v:quiet /nologo"
$projectDir = GetProjectDirectory($ProjectFilePath)
$appName = GetApplicationName
$newExeVersion = Get-ChildItem .\bin\$Configuration\$appName.exe | Select-Object -ExpandProperty VersionInfo | % { $_.FileVersion }

Write-Host $outputPrefix"Building ClickOnce installer..."
#
# Because the ClickOnce target doesn't automatically update or sync the application version
# with the assembly version of the EXE, we need to grab the version off of the built assembly
# and update the Executable.csproj file with the new application version.
#
$ProjectXml = [xml](Get-Content $ProjectFilePath)
$ns = new-object Xml.XmlNamespaceManager $ProjectXml.NameTable
$ns.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
$AppVersion = $ProjectXml.SelectSingleNode("//msb:Project/msb:PropertyGroup/msb:ApplicationVersion", $ns)
$AppVersion.InnerText = $newExeVersion
$TargetPath = Resolve-Path $ProjectFilePath
$ProjectXml.Save($TargetPath)

Invoke-Expression "$msbuild $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:publish /v:quiet /nologo"

Write-Host $outputPrefix"Deploying updates to network server..."
$LocalInstallerPath = (Resolve-Path "$projectDir\bin\$Configuration\app.publish").ToString() + "\*"
Copy-Item $LocalInstallerPath $DeployPath -Recurse -Force
