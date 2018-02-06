#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Figlet&version=1.0.0"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Npx&version=1.0.0"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projectName = "Cake.Npx";
var releaseVersion = "0.0.0";
var artifactsDir =  Directory("./artifacts");

var isLocalBuild = BuildSystem.IsLocalBuild;
var isRunningOnAppveyorMasterBranch = StringComparer.OrdinalIgnoreCase.Equals(
    "master",
    BuildSystem.AppVeyor.Environment.Repository.Branch
);
var shouldRelease = !isLocalBuild && isRunningOnAppveyorMasterBranch;
var changesDetectedSinceLastRelease = false;

Action<NpxSettings> requiredSemanticVersionPackages = settings => settings
    .AddPackage("semantic-release@13.1.1")
    .AddPackage("@semantic-release/changelog@1.0.0")
    .AddPackage("@semantic-release/git@3.0.0")
    .AddPackage("@semantic-release/exec@2.0.0");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information(Figlet(projectName));
    Information("Local build {0}", isLocalBuild);
    Information("Running on appveyor master branch {0}", isRunningOnAppveyorMasterBranch);
    Information("Should release {0}", shouldRelease);
});

Teardown(context =>
{
    Information("Finished running tasks ✔");
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("Build")
    .IsDependentOn("Run dotnet --info")
    .IsDependentOn("Clean")
    .IsDependentOn("Get next semantic version number")
    .IsDependentOn("Build solution")
    .IsDependentOn("Run tests")
    .IsDependentOn("Package")
    .IsDependentOn("Release")
    ;

Task("Run dotnet --info")
    .Does(() =>
{
    Information("dotnet --info");
    StartProcess("dotnet", new ProcessSettings { Arguments = "--info" });
});

Task("Clean")
    .Does(() =>
{
    Information("Cleaning {0}, bin and obj folders", artifactsDir);

    CleanDirectory(artifactsDir);
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

/*
Normally this task should only run based on the 'shouldRelease' condition,
however sometimes you want to run this locally to preview the next sematic version
number and changlelog.

To do this run the following locally:
> $env:NUGET_TOKEN="insert_token_here"
> $env:GITHUB_TOKEN="insert_token_here"
> .\build.ps1  -ScriptArgs '-target="Get next semantic version number"'

NOTE: The GITHUB_TOKEN environment variable will need to be set
so that semantic-release can access the repository

Explicitly setting the target will override the 'shouldRelease' condition
*/
Task("Get next semantic version number")
    .WithCriteria(shouldRelease || target == "Get next semantic version number" )
    .Does(() =>
{
    Information("Running semantic-release in dry run mode to extract next semantic version number");

    string[] semanticReleaseOutput;
    Npx("semantic-release", "--dry-run", requiredSemanticVersionPackages, out semanticReleaseOutput);

    Information(string.Join(Environment.NewLine, semanticReleaseOutput));

    var nextSemanticVersionNumber = ExtractNextSemanticVersionNumber(semanticReleaseOutput);

    if (nextSemanticVersionNumber == null) {
        Warning("There are no relevant changes, skipping release");
    } else {
        Information("Next semantic version number is {0}", nextSemanticVersionNumber);
        releaseVersion = nextSemanticVersionNumber;
        changesDetectedSinceLastRelease = true;
    }
});

Task("Build solution")
    .Does(() =>
{
    var solutions = GetFiles("./src/*.sln");
    foreach(var solution in solutions)
    {
        Information("Building solution {0} v{1}", solution.GetFilenameWithoutExtension(), releaseVersion);

        var assemblyVersion = $"{releaseVersion}.0";

        DotNetCoreBuild(solution.FullPath, new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .WithProperty("Version", assemblyVersion)
                .WithProperty("AssemblyVersion", assemblyVersion)
                .WithProperty("FileVersion", assemblyVersion)
                // 0 = use as many processes as there are available CPUs to build the project
                // see: https://develop.cakebuild.net/api/Cake.Common.Tools.MSBuild/MSBuildSettings/60E763EA
                .SetMaxCpuCount(0)
        });
    }
});

Task("Run tests")
    .Does(() =>
{
    var xunitArgs = "-nobuild -configuration " + configuration;

    var testProjects = GetFiles("./src/**/*.Tests.csproj");
    foreach(var testProject in testProjects)
    {
        Information("Testing project {0} with args {1}", testProject.GetFilenameWithoutExtension(), xunitArgs);

        DotNetCoreTool(testProject.FullPath, "xunit", xunitArgs);
    }
});

Task("Package")
    .Does(() =>
{
    var projects = GetFiles("./src/**/*.csproj");
    foreach(var project in projects)
    {
        var projectDirectory = project.GetDirectory().FullPath;
        if(projectDirectory.EndsWith("Tests")) continue;

        Information("Packaging project {0} v{1}", project.GetFilenameWithoutExtension(), releaseVersion);

        var assemblyVersion = $"{releaseVersion}.0";

        DotNetCorePack(project.FullPath, new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = artifactsDir,
            NoBuild = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .WithProperty("Version", assemblyVersion)
                .WithProperty("AssemblyVersion", assemblyVersion)
                .WithProperty("FileVersion", assemblyVersion)
        });
    }
});

Task("Release")
    .WithCriteria(shouldRelease)
    .WithCriteria(() => changesDetectedSinceLastRelease)
    .Does(() =>
{
    Information("Releasing v{0}", releaseVersion);
    Information("Updating CHANGELOG.md");
    Information("Creating github release");
    Information("Pushing to NuGet");

    Npx("semantic-release", requiredSemanticVersionPackages);
});

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

///////////////////////////////////////////////////////////////////////////////
// Helpers
///////////////////////////////////////////////////////////////////////////////

string ExtractNextSemanticVersionNumber(string[] semanticReleaseOutput)
{
    var extractRegEx = new System.Text.RegularExpressions.Regex("^.+next release version is (?<SemanticVersionNumber>.*)$");

    return semanticReleaseOutput
        .Select(line => extractRegEx.Match(line).Groups["SemanticVersionNumber"].Value)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .SingleOrDefault();
}