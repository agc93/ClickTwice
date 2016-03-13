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

function GetProgramFilesPath() {
	$os_type = (Get-WmiObject -Class Win32_ComputerSystem).SystemType -match '(x64)'
	If (-not ${env:ProgramFiles(x86)}) {
		$programFilesPath = $env:ProgramFiles
	} Else {
		$programFilesPath = ${env:ProgramFiles(x86)}
	}
	return $programFilesPath
}

function CheckMSBuildPath ()
{
	$path = GetProgramFilesPath
	If (-not $env:msbuild) {
		if (Test-Path "$path\MSBuild\14.0") {
			$msbuild = "$path\MSBuild\14.0\Bin\MSBuild.exe"
		} else {
			if (Test-Path "$path\MSBuild\12.0") {
				$msbuild = "$path\MSBuild\12.0\Bin\MSBuild.exe"
			} else {
				$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.EXE" 
			}
		}
	} Else {
		$msbuild = $env:msbuild
	}
	return $msbuild
}

function GetProjectDirectory ($projectFile)
{
	return Get-ChildItem $ProjectFilePath | % {$_.DirectoryName}
}

function GetApplicationName() {
	[string]$projectName = Get-ChildItem $ProjectFilePath | % {$_.BaseName}
	return $projectName
}
$msbuild = CheckMSBuildPath
Write-Host "Using MSBuild from $msbuild"
Write-Host $outputPrefix"Cleaning the build directory..."
Invoke-Expression "& '$msbuild' $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:clean /v:quiet /nologo"

Write-Host $outputPrefix"Building Executable application..."
Invoke-Expression "& '$msbuild' $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:build /v:quiet /nologo"
$projectDir = GetProjectDirectory($ProjectFilePath)
$appName = GetApplicationName
$newExeVersion = Get-ChildItem $projectDir\bin\$Configuration\$appName.exe | Select-Object -ExpandProperty VersionInfo | % { $_.FileVersion }

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

Invoke-Expression "& '$msbuild' $ProjectFilePath /p:Configuration=$Configuration /p:Platform=AnyCPU /t:publish /v:quiet /nologo"

Write-Host $outputPrefix"Deploying application files to target..."
$LocalInstallerPath = (Resolve-Path "$projectDir\bin\$Configuration\app.publish").ToString() + "\*"
Copy-Item $LocalInstallerPath $DeployPath -Recurse -Force
