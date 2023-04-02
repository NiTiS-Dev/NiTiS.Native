using NiTiS.Native.NUKE;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Octokit;
using Octokit.Internal;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.Git.GitTasks;

namespace NiTiS.Native.NUKE;

partial class Build
{
	static AbsolutePath SDLPath => RootDirectory / "build" / "submodules" / "SDL";
	static AbsolutePath SDLRuntimes => RootDirectory / "src" / "NiTiS.Native.SDL.Container" / "runtimes";

	Target SDL => _ => _
		.Before(Compile)
		.After(Clean)
		.Executes(
			() =>
			{
				var @out = SDLPath / "build";
				var prepare = "cmake -S.. -B . -D CMAKE_BUILD_TYPE=Release";
				var build = $"cmake --build . --config Release --parallel";

				EnsureCleanDirectory(@out);

				if (OperatingSystem.IsWindows())
				{
					InheritedShell($"{prepare} -DCMAKE_GENERATOR_PLATFORM=X64", @out)
						.AssertZeroExitCode();
					InheritedShell(build, @out)
						.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("Release/SDL*.dll"), SDLRuntimes / ProcKind.WinX64);

					EnsureCleanDirectory(@out);

					InheritedShell($"{prepare} -DCMAKE_GENERATOR_PLATFORM=Win32", @out)
						.AssertZeroExitCode();
					InheritedShell(build, @out)
					.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("Release/SDL*.dll"), SDLRuntimes / ProcKind.WinX32);

					//EnsureCleanDirectory(@out);

					//InheritedShell($"{prepare} -DCMAKE_GENERATOR_PLATFORM=ARM64", @out)
					//	.AssertZeroExitCode();
					//InheritedShell(build, @out)
					//	.AssertZeroExitCode(); // SDK does not support arm64 processor yet .･ﾟﾟ･(／ω＼)･ﾟﾟ･.

					//CopyAll(@out.GlobFiles("Release/SDL3.dll"), GLFWRuntimes / ProcKind.WinARM64);
					Logger.Warn("Generation SDL::WinARM64 is canceled (GLFW dosnt support build for WinARM64)");
				}
				else if (OperatingSystem.IsLinux())
				{
					InheritedShell($"{prepare} -DCMAKE_SYSTEM_PROCESSOR=x86_64", @out)
						.AssertZeroExitCode();
					InheritedShell(build, @out)
						.AssertZeroExitCode();
					CopyAll(@out.GlobFiles("src/libSDL*.so"), SDLRuntimes / ProcKind.LinuxX64);
				}
				else if (OperatingSystem.IsMacOS())
				{
					InheritedShell($"{prepare} -DCMAKE_OSX_ARCHITECTURES=x86_64", @out)
						.AssertZeroExitCode();
					InheritedShell(build, @out)
						.AssertZeroExitCode();
					CopyAll(@out.GlobFiles("src/libSDL*.dylib"), SDLRuntimes / ProcKind.osxX64);

					EnsureCleanDirectory(@out);

					InheritedShell($"{prepare} -DCMAKE_OSX_ARCHITECTURES=arm64", @out)
						.AssertZeroExitCode();
					InheritedShell(build, @out)
						.AssertZeroExitCode();

					CopyAll(@out.GlobFiles("src/libSDL*.dylib"), SDLRuntimes / ProcKind.osxARM64);
				}

				PrUpdatedNativeBinary("GLFW");
			}
		);
}