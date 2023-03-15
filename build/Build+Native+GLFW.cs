using Nuke.Common;
using Nuke.Common.IO;
using System;
using static Nuke.Common.Tools.NSwag.NSwagTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tooling;
using NiTiS.Native.NUKE;
using Nuke.Common.CI.GitHubActions;
using Octokit.Internal;
using Octokit;
using System.Linq;
using System.Runtime.InteropServices;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
	static AbsolutePath GLFWPath => RootDirectory / "build" / "submodules" / "GLFW";
	static AbsolutePath GLFWRuntimes => RootDirectory / "src" / "NiTiS.Native.GLFW.Container" / "runtimes";

	Target GLFW => _ => _
		.Before(Compile)
		.After(Clean)
		.Executes(
			() =>
			{
				var @out = GLFWPath / "build";
				var prepare = "cmake -S. -B build -D BUILD_SHARED_LIBS=ON";
				var build = $"cmake --build build --config Release";

				EnsureCleanDirectory(@out);

				if (OperatingSystem.IsWindows())
				{
					InheritedShell($"{prepare} -A X64", GLFWPath)
						.AssertZeroExitCode();
					InheritedShell(build, GLFWPath)
						.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("src/Release/glfw3.dll"), GLFWRuntimes / RuntimeKind.WinX64);

					EnsureCleanDirectory(@out);

					InheritedShell($"{prepare} -A Win32", GLFWPath)
						.AssertZeroExitCode();
					InheritedShell(build, GLFWPath)
					.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("src/Release/glfw3.dll"), GLFWRuntimes / RuntimeKind.WinX32);

					EnsureCleanDirectory(@out);

					InheritedShell($"{prepare} -A arm64", GLFWPath);
					InheritedShell(build, GLFWPath); // GLFW does not support arm64 processor yet .･ﾟﾟ･(／ω＼)･ﾟﾟ･.

					CopyAll(@out.GlobFiles("src/Release/glfw3.dll"), GLFWRuntimes / RuntimeKind.WinARM64);
				}
				else if (OperatingSystem.IsLinux())
				{
					InheritedShell($"{prepare} -DCMAKE_SYSTEM_PROCESSOR=x86_64", GLFWPath)
						.AssertZeroExitCode();
					InheritedShell(build, GLFWPath)
						.AssertZeroExitCode();
					CopyAll(@out.GlobFiles("src/libglfw.so"), GLFWRuntimes / RuntimeKind.LinuxX64);
				}
				else if (OperatingSystem.IsMacOS())
				{
					InheritedShell($"{prepare} -DCMAKE_OSX_ARCHITECTURES=x86_64", GLFWPath)
						.AssertZeroExitCode();
					InheritedShell(build, GLFWPath)
						.AssertZeroExitCode();
					CopyAll(@out.GlobFiles("src/libglfw.3.dylib"), GLFWRuntimes / RuntimeKind.osxX64);

					EnsureCleanDirectory(@out);

					InheritedShell($"{prepare} -DCMAKE_OSX_ARCHITECTURES=arm64", GLFWPath)
						.AssertZeroExitCode();
					InheritedShell(build, GLFWPath)
						.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("src/libglfw.3.dylib"), GLFWRuntimes / RuntimeKind.osxARM64);
				}

				PrUpdatedNativeBinary("GLFW");
			}
		);

	void PrUpdatedNativeBinary(string name)
	{
		var pushableToken = EnvironmentInfo.GetVariable<string>("GITHUB_TOKEN");
		var curBranch = GitCurrentBranch(RootDirectory);
		if (!string.IsNullOrWhiteSpace(pushableToken) &&
			GitHubActions.Instance?.Repository == "NiTiS-Dev/NiTiS.Native" &&
			curBranch != "HEAD" &&
			!string.IsNullOrWhiteSpace(curBranch) &&
			!curBranch.StartsWith("ci/", StringComparison.OrdinalIgnoreCase) && // ignore other CI branches
			!curBranch.StartsWith("main", StringComparison.OrdinalIgnoreCase) && // submodule updates are done in PRs...
			!curBranch.StartsWith("feature/", StringComparison.OrdinalIgnoreCase))
		{
			// it's assumed that the pushable token was used to checkout the repo
			var suffix = string.Empty;
			if (OperatingSystem.IsWindows())
			{
				suffix = "/**/*.dll";
			}
			else if (OperatingSystem.IsMacOS())
			{
				suffix = "/**/*.dylib";
			}
			else if (OperatingSystem.IsLinux())
			{
				suffix = "/**/*.so*";
			}

			Git("fetch --all", RootDirectory);
			Git("pull");
			Git($"add src{suffix}", RootDirectory);
			var newBranch = $"ci/{curBranch}/{name.ToLower().Replace(' ', '_')}_bins";
			var curCommit = GitCurrentCommit(RootDirectory);
			var commitCmd = InheritedShell
				(
					$"git commit -m \"New binaries for {name} on {RuntimeInformation.OSDescription}\""
				)
				.AssertWaitForExit();
			if (!commitCmd.Output.Any(x => x.Text.Contains("nothing to commit", StringComparison.OrdinalIgnoreCase)))
			{
				commitCmd.AssertZeroExitCode();
			}

			// ensure there are no other changes
			Git("checkout HEAD .nuke/", RootDirectory);
			Git("reset --hard", RootDirectory);
			if (GitCurrentCommit(RootDirectory) != curCommit) // might get "nothing to commit", you never know...
			{
				Logger.Info("Checking for existing branch...");
				var exists = StartProcess("git", $"checkout \"{newBranch}\"", RootDirectory)
					.AssertWaitForExit()
					.ExitCode == 0;
				if (!exists)
				{
					Logger.Info("None found, creating a new one...");
					Git($"checkout -b \"{newBranch}\"");
				}

				Git($"merge -X theirs \"{curBranch}\" --allow-unrelated-histories");
				Git($"push --set-upstream origin \"{newBranch}\"");
				if (!exists)
				{
					var github = new GitHubClient
					(
						new ProductHeaderValue("NiTiS-Dev"),
						new InMemoryCredentialStore(new Credentials(pushableToken))
					);

					var pr = github.PullRequest.Create
							("NiTiS-Dev", "NiTIS.Native", new($"Update {name} binaries", newBranch, curBranch))
						.GetAwaiter()
						.GetResult();
				}
			}
		}
	}
}