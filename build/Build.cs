using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution]
	readonly Solution Solution;

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
		});

	Target Restore => _ => _
		.Executes(() =>
		{
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.DependsOn(GenerateBindings)
		.Produces("output/compile/**/*.dll")
		.Executes(() =>
		{
			DotNetBuildSettings setting = new();
			setting.SetProjectFile(Solution);
			setting.EnableNoRestore();
			setting.SetConfiguration(Configuration);
			setting.SetOutputDirectory(Solution.Directory / "output" / "compile");

			using IProcess proc = StartProcess(setting);
			proc.AssertZeroExitCode();
		});
	Target Pack => _ => _
		.DependsOn(Restore)
		.After(Clean, GenerateBindings)
		.Produces("output/packages/*.nupkg")
		.Executes(() =>
		{
			DotNetPackSettings setting = new();
			setting.EnableNoRestore();
			setting.SetConfiguration(Configuration);

			using IProcess proc = StartProcess(setting);
			proc.AssertZeroExitCode();
		});
}