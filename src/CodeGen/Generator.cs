using CodeGen.Analyzers;
using CodeGen.Signature;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGen;

public static class Generator
{
	private static object console_l = new();
	public static void Run(Dictionary<string, CodeGenTask> tasks)
	{
		bool isParallel = !Environment.GetCommandLineArgs().Contains("--no-parallel");

		if (isParallel)
		{
			Parallel.ForEach(tasks, (step, state) =>
			{
				SafeConsoleWriteLine($"===BEGIN={step.Key.ToUpperInvariant()}=TASK===");
				RunTask(step.Value, step.Key);
				SafeConsoleWriteLine($"===FINISH={step.Key.ToUpperInvariant()}=TASK===");
			});
		}
		else
		{
			foreach ((string name, CodeGenTask task) in tasks)
			{
				SafeConsoleWriteLine($"===BEGIN={name.ToUpperInvariant()}=TASK===");
				RunTask(task, name);
				SafeConsoleWriteLine($"===FINISH={name.ToUpperInvariant()}=TASK===");
			}
		}
	}

	private static void SafeConsoleWriteLine(string text)
	{
		lock (console_l)
		{
			Console.WriteLine(text);
		}
	}

	private static void RunTask(CodeGenTask task, string taskName)
	{
		if (task.Analyzer is null)
		{
			SafeConsoleWriteLine("Analyzer not specified");
			return;
		}

		Analyzer analyzer = AnalyzerDome.GetByName(task.Analyzer!.Name ?? "!!unspecified!!");

		SignatureUnit unit = new();

		for (int i = 0; i < task?.IncludeFiles?.Count; i++)
		{
			string filePath = task.IncludeFiles[i];
			analyzer.Analyze(File.ReadAllText(filePath), unit);

			SafeConsoleWriteLine($"Analyzed file {taskName}::{Path.GetFileName(filePath)}");
		}

		if (unit.LicenseContent is not null && task?.Output?.License is not null)
		{
			SafeConsoleWriteLine($"===SAVE={taskName.ToUpperInvariant()}=LICENSE===");

			string licensePath = Path.Combine(task!.Output!.TargetDirectory!, task!.Output!.License!);
			File.WriteAllText(licensePath, unit.LicenseContent);

			SafeConsoleWriteLine($"License created: {licensePath}");
		}
	}
}