#l "scripts/utilities.cake"
#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
#addin "Cake.FileHelpers"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var paths = new {
    solution = MakeAbsolute(File("./../StringFormatter.sln")).FullPath,
    mainProject = MakeAbsolute(File("./../StringFormatter/StringFormatter.csproj")).FullPath,
    testProject = MakeAbsolute(File("./../Test/Test.csproj")).FullPath,
    version = MakeAbsolute(File("./../version.yml")).FullPath,
    assemblyInfo = MakeAbsolute(File("./../SharedVersionInfo.cs")).FullPath,
    output = new {
        build = MakeAbsolute(Directory("./../output/build")).FullPath,
        nuget = MakeAbsolute(Directory("./../output/nuget")).FullPath,
    },
    nuspec = MakeAbsolute(File("./StringFormatter.nuspec")).FullPath,
};

ReadContext(paths.version);

//////////////////////////////////////////////////////////////////////
// HELPERS
//////////////////////////////////////////////////////////////////////

private void BuildMainProject(string configuration, string outputDirectoryPath)
{
    MSBuild(paths.mainProject, settings => settings.SetConfiguration(configuration)
                                                .SetPlatformTarget(PlatformTarget.MSIL)
                                                .UseToolVersion(MSBuildToolVersion.VS2017)
                                                .WithProperty("BaseOutputPath", outputDirectoryPath));
}

private void Build(string configuration, string outputDirectoryPath)
{
    MSBuild(paths.solution, settings => settings.SetConfiguration(configuration)
                                                .SetPlatformTarget(PlatformTarget.MSIL)
                                                .UseToolVersion(MSBuildToolVersion.VS2017)
                                                .WithProperty("OutDir", outputDirectoryPath + "/" + configuration));
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("UpdateBuildVersionNumber").Does(() => UpdateAppVeyorBuildVersionNumber());
Task("Clean").Does(() =>
{
    CleanDirectory(paths.output.build);
    CleanDirectory(paths.output.nuget);
});
Task("Restore-NuGet-Packages").Does(() => NuGetRestore(paths.solution));
Task("Create-AssemblyInfo").Does(()=>{
    CreateAssemblyInfo(paths.assemblyInfo, new AssemblyInfoSettings {
        Version = VersionContext.AssemblyVersion,
        FileVersion = VersionContext.AssemblyVersion,
        InformationalVersion = VersionContext.NugetVersion + " Commit: " + VersionContext.Git.Sha
    });
});
Task("Build-Debug").Does(() => Build("Debug", paths.output.build));
Task("Build-Release").Does(() => Build("Release", paths.output.build));
Task("Build-Main-Project-Release").Does(() => BuildMainProject("Release", paths.output.build));
Task("Clean-AssemblyInfo").Does(() => FileWriteText(paths.assemblyInfo, string.Empty));
Task("Run-Debug-Unit-Tests").Does(() => DotNetCoreTest(paths.testProject, new DotNetCoreTestSettings { Configuration = "Debug", Framework = "net45" }));
Task("Run-Release-Unit-Tests").Does(() => DotNetCoreTest(paths.testProject, new DotNetCoreTestSettings { Configuration = "Release", Framework = "net45" }));
Task("Nuget-Pack").Does(() => 
{
    NuGetPack(paths.nuspec, new NuGetPackSettings {
        Version = VersionContext.NugetVersion,
        BasePath = paths.output.build,
        OutputDirectory = paths.output.nuget
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("UpdateBuildVersionNumber")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Create-AssemblyInfo")
    .IsDependentOn("Build-Debug")
    .IsDependentOn("Build-Release")
    .IsDependentOn("Build-Main-Project-Release")
    .IsDependentOn("Clean-AssemblyInfo");

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Debug-Unit-Tests")
    .IsDependentOn("Run-Release-Unit-Tests");

Task("Nuget")
    .IsDependentOn("Test")
    .IsDependentOn("Nuget-Pack")
    .Does(() => {
        Information("   Nuget package is now ready at location: {0}.", paths.output.nuget);
        Warning("   Please remember to create and push a tag based on the currently built version.");
        Information("   You can do so by copying/pasting the following commands:");
        Information("       git tag v{0}", VersionContext.NugetVersion);
        Information("       git push origin --tags");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
