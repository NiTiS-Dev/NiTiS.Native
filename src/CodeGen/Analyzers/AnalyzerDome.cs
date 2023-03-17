using System;

namespace CodeGen.Analyzers;

public static class AnalyzerDome
{
	private static GLFWAnalyzer glfw;
	public static Analyzer GetByName(string name)
	{
		return name.ToLowerInvariant() switch
		{
			"glfw" or "glfwanalyzer" => glfw ??= new(),
			_ => throw new ArgumentException(name, nameof(name))
		};
	}
}
