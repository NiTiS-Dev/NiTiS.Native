using JetBrains.Annotations;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{

	void AddToPath(string dir)
	{
		var pathVar = Environment.GetEnvironmentVariables()
			.Cast<DictionaryEntry>()
			.First(x => ((string)x.Key).Equals("PATH", StringComparison.OrdinalIgnoreCase));
		Environment.SetEnvironmentVariable
		(
			(string)pathVar.Key,
			(string)pathVar.Value + (OperatingSystem.IsWindows() ? $";{dir}" : $":{dir}")
		);
	}

	static void CopyAll(IEnumerable<AbsolutePath> paths, AbsolutePath dir)
	{
		foreach (var path in paths)
		{
			CopyFile(path, dir / Path.GetFileName(path), FileExistsPolicy.Overwrite);
		}
	}

	Dictionary<string, string> CreateEnvVarDictionary()
	=> Environment.GetEnvironmentVariables()
		.Cast<DictionaryEntry>()
		.ToDictionary(x => (string)x.Key, x => (string)x.Value);

	IProcess InheritedShell(string cmd, [CanBeNull] string workDir = null)
	=> OperatingSystem.IsWindows()
		? StartProcess("powershell", $"-Command {cmd.DoubleQuote()}", workDir, CreateEnvVarDictionary())
		: StartProcess("bash", $"-c {cmd.DoubleQuote()}", workDir, CreateEnvVarDictionary());
}
