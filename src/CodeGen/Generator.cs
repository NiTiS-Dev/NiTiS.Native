using CodeGen.Analyzers;
using CodeGen.Generators;
using CodeGen.Signature;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeGen;

public static class Generator
{
	private static object consoleLock = new();
	private static HttpClient? httpClient;
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
		lock (consoleLock)
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

		CompilationSignature sign = new();

		for (int i = 0; i < task?.IncludeFiles?.Count; i++)
		{
			string filePath = task.IncludeFiles[i];

			if (Uri.TryCreate(filePath, new UriCreationOptions(), out Uri? result) && !result.IsFile)
			{
				httpClient ??= new();
				string tempFile = Path.GetTempFileName();

				using Stream contentStream = httpClient.GetStreamAsync(result).Result;
				using FileStream fs = File.OpenWrite(tempFile);
				contentStream.CopyTo(fs);

				filePath = tempFile;
				task.IncludeFiles[i] = tempFile;

				fs.Flush();
			}

			analyzer.Analyze(task, File.ReadAllText(filePath), sign);

			SafeConsoleWriteLine($"Analyzed file {taskName}::{Path.GetFileName(filePath)}");
		}

		if (sign.LicenseContent is not null && task?.Output?.License is not null)
		{
			SafeConsoleWriteLine($"===SAVE={taskName.ToUpperInvariant()}=LICENSE===");

			string licensePath = Path.Combine(task!.Output!.TargetDirectory!, task!.Output!.License!);
			File.WriteAllText(licensePath, sign.LicenseContent);

			SafeConsoleWriteLine($"License created: {licensePath}");
		}

		TypeGenerator typeGen = new(task);
		foreach (BasicTypeSignature typeSign in sign.Types)
		{
			typeGen.PreInitialize(typeSign);
		}
		foreach (BasicTypeSignature typeSign in sign.Types)
		{
			typeGen.Generate(typeSign);
		}
	}
}