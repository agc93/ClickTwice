#tool "ILRepack"
#tool "GitVersion.CommandLine"
#load "extensions.cake"
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/ClickTwice.sln");
var solution = ParseSolution(solutionPath);
var projects = solution.Projects;
var projectPaths = projects.Select(p => p.Path.GetDirectory());
//var testAssemblies = projects.Where(p => p.Name.Contains("Tests")).Select(p => p.Path.GetDirectory() + "/bin/" + configuration + "/" + p.Name + ".dll");
var artifacts = "./artifacts/";
//var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
GitVersion versionInfo = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
	versionInfo = GitVersion();
	Information("Building for version {0}", versionInfo.FullSemVer);
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in projectPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
    Information("Cleaning common files...");
    CleanDirectory(artifacts);
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    Information("Restoring solution...");
    NuGetRestore(solutionPath);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	Information("Building solution...");
	MSBuild(solutionPath, settings =>
		settings.SetPlatformTarget(PlatformTarget.MSIL)
			//.WithProperty("TreatWarningsAsErrors","true")
			.SetVerbosity(Verbosity.Quiet)
			.WithTarget("Build")
			.SetConfiguration(configuration));
});

Task("Copy-Files")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Libs")
    .IsDependentOn("Copy-Scripts")
    .Does(() =>
{
    CreateDirectory(artifacts + "build");
	foreach (var project in projects) {
		CreateDirectory(artifacts + "build/" + project.Name);
		var files = GetFiles(project.Path.GetDirectory() +"/bin/" +configuration +"/" +project.Name +".*");
		CopyFiles(files, artifacts + "build/" + project.Name);
	}
});

Task("Copy-Libs")
    .IsDependentOn("Build")
    .Does(() => {
        CreateDirectory(artifacts + "lib");
	    foreach (var project in projects) {
		    CreateDirectory(artifacts + "lib/" + project.Name);
		    var files = GetFiles(project.Path.GetDirectory() +"/bin/" +configuration +"/*.dll");
		    CopyFiles(files, artifacts + "lib/" + project.Name);
	    }
    });

Task("Copy-Scripts")
    .IsDependentOn("Copy-Libs")
    .Does(() => {
        var scriptDir = artifacts + "/scripts/";
        CreateDirectory(scriptDir);
        CreateDirectory(scriptDir + "/bin/");
        var files = GetFiles(artifacts + "lib/ScriptCs.ClickTwice/*ClickTwice*.dll");
        CopyFiles(files, scriptDir + "/bin/");
        CopyFiles(GetFiles("./*.csx"), artifacts + "/scripts/");
    });

Task("Publish")
	.IsDependentOn("Build")
	.IsDependentOn("Copy-Files")
	.Does(() => {
        var mergeProjects = new[] {"ClickTwice.Handlers.LaunchPage", "ClickTwice.Handlers.AppDetailsPage"};
        foreach (var project in mergeProjects) {
		Information("Merging libraries");
		var assemblyList = GetFiles("./src/" + project + "/bin/" + configuration + "/**/*.dll");
		Information("Executing ILMerge to merge {0} assemblies", assemblyList.Count);
		ILRepack(
			artifacts + project + ".dll",
			"./src/" + project + "/bin/" + configuration + "/" + project + ".dll",
			assemblyList);
        }
	});
    
    Task("Merge")
    .IsDependentOn("Copy-Files")
    .Does(() => {
        var assemblyList = GetFiles("./src/ClickTwice.Publisher.Core/bin/" + configuration + "/**/*ClickTwice*.dll").ToList();
        //AddIfNotExists(assemblyList, GetFiles("./src/ClickTwice.Handlers.LaunchPage/bin/" + configuration + "/**/*.dll"));
        AddIfNotExists(assemblyList, GetFiles("./src/ClickTwice.Handlers.AppDetailsPage/bin/" + configuration + "/**/*.dll"));
        foreach(var assembly in assemblyList) {
            Information("Merging: {0}", assembly.ToString());
        }
        Information("Merging {0} assemblies", assemblyList.Count);
        ILRepack(
            artifacts + "ClickTwice.dll",
            "./src/ClickTwice.Publisher.Core/bin/" + configuration + "/ClickTwice.Publisher.Core.dll",
            assemblyList,
            new ILRepackSettings {
                Union = true,
                CopyAttrs = true,
                AllowMultiple = true,
                XmlDocs = true
            });
    });

    Task("Post-Build")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Files")
    .IsDependentOn("Merge");

    Task("NuGet")
    .IsDependentOn("Post-Build")
    .Does(() => {
        CreateDirectory(artifacts + "package/");
        CreateDirectory(artifacts + "templates/");
        var specFiles = GetFiles("./**/*.nuspec");
		var nuspecFiles = specFiles.Where(f => !f.FullPath.Contains("Template"));
        var templateFiles = specFiles.Where(f => f.FullPath.Contains("Template"));
        Information("Building {0} NuGet package(s)", specFiles.Count());
        Verbose("Building: {0}{1}", Environment.NewLine, string.Join(Environment.NewLine, templateFiles));
        Information("Building {0} library packages and {1} template packages", nuspecFiles.Count(), templateFiles.Count());
		var versionNotes = ParseAllReleaseNotes("./ReleaseNotes.md").FirstOrDefault(v => v.Version.ToString() == versionInfo.MajorMinorPatch);
		NuGetPack(nuspecFiles, new NuGetPackSettings() {
			Version = versionInfo.NuGetVersionV2,
			ReleaseNotes = versionNotes != null ? versionNotes.Notes.ToList() : new List<string>(),
			OutputDirectory = artifacts + "/package",
            BasePath = artifacts
			});
        NuGetPack(templateFiles, new NuGetPackSettings {
            Version = versionInfo.NuGetVersionV2,
			OutputDirectory = artifacts + "/templates",
        });
    });

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
