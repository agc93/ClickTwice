#
# AutoPublish.ps1
#
Param (
	[string]$ProjectFilePath,
	[string]$Configuration
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
	[string]$extension = 
	$projectName.Substring(0, $projectName.Length, 
}
CheckMSBuildPath()
Write-Host $outputPrefix"Cleaning the build directory..."
Invoke-Expression "$msbuild $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:clean /v:quiet /nologo"

Write-Host $outputPrefix"Building Executable application..."
Invoke-Expression "$msbuild $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:build /v:quiet /nologo"

$newExeVersion = Get-ChildItem .\Executable\bin\Release\Executable.exe | Select-Object -ExpandProperty VersionInfo | % { $_.FileVersion }
$newLibVersion = Get-ChildItem .\Executable\bin\Release\Library.dll | Select-Object -ExpandProperty VersionInfo | % { $_.FileVersion }

Write-Host $outputPrefix"Building ClickOnce installer..."
#
# Because the ClickOnce target doesn't automatically update or sync the application version
# with the assembly version of the EXE, we need to grab the version off of the built assembly
# and update the Executable.csproj file with the new application version.
#
$ProjectXml = [xml](Get-Content Executable\Executable.csproj)
$ns = new-object Xml.XmlNamespaceManager $ProjectXml.NameTable
$ns.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
$AppVersion = $ProjectXml.SelectSingleNode("//msb:Project/msb:PropertyGroup/msb:ApplicationVersion", $ns)
$AppVersion.InnerText = $newExeVersion
$TargetPath = Resolve-Path "Executable\Executable.csproj"
$ProjectXml.Save($TargetPath)

Invoke-Expression "$msbuild Executable\Executable.csproj /p:Configuration=Release /p:Platform=AnyCPU /t:publish /v:quiet /nologo"

Write-Host $outputPrefix"Deploying updates to network server..."
$LocalInstallerPath = (Resolve-Path "Executable\bin\Release\app.publish").ToString() + "\*"
$RemoteInstallerPath = "\\network\path\Executable\DesktopClient\"
Copy-Item $LocalInstallerPath $RemoteInstallerPath -Recurse -Force

Write-Host $outputPrefix"Committing version increments to Perforce..."
p4 submit -d "Updating Executable ClickOnce Installer to version $newExeVersion" //my/project/tool/path/Executable/Executable.csproj | Out-Null
p4 submit -d "Updating Library to version $newLibVersion" //my/project/tool/path/Library/Properties/AssemblyInfo.cs | Out-Null
p4 submit -d "Updating Executable to version $newExeVersion" //my/project/tool/path/Executable/Properties/AssemblyInfo.cs | Out-Null
