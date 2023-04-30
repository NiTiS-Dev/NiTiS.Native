using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nuke.Common.Tooling.ProcessTasks;

namespace NiTiS.Native.NUKE;
partial class Build
{
	static string PackageDirectory => RootDirectory / "output" / "packages";
	static IEnumerable<string> Packages => Directory.GetFiles(PackageDirectory, "*.nupkg")
		.Where(x => Path.GetFileName(x).StartsWith("NiTiS"));

	Target PushToAll => _ => _
		.DependsOn(PushNuGet, PushGithub);

	Target PushNuGet => _ => _
		.DependsOn(Pack)
		.Executes(() =>
		{
			foreach (var package in Packages)
			{
				DotNetNuGetPushSettings push = new DotNetNuGetPushSettings()
					.SetSkipDuplicate(true)
					.SetApiKey(Environment.GetEnvironmentVariable("NUGET_TOKEN"))
					.SetTargetPath(package)
					.SetSource("https://api.nuget.org/v3/index.json");

				Log.Information("NugetPush: {0}", package);

				using IProcess proc = StartProcess(push);
				proc.AssertZeroExitCode();
			}
		});

	Target PushGithub => _ => _
		.DependsOn(Pack)
		.Executes(() =>
		{
			foreach (var package in Packages)
			{
				DotNetNuGetPushSettings push = new DotNetNuGetPushSettings()
					.SetSkipDuplicate(true)
					.SetApiKey(Environment.GetEnvironmentVariable("GITHUB_TOKEN"))
					.SetTargetPath(package)
					.SetSource("https://nuget.pkg.github.com/NiTiS-Dev/index.json");

				Log.Information("GitHubPush: {0}", package);

				using IProcess proc = StartProcess(push);
				proc.AssertZeroExitCode();
			}
		});
}