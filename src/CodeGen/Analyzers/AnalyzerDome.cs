using System;

namespace CodeGen.Analyzers;

public static class AnalyzerDome
{
	private static GLFWAnalyzer glfw;
	private static GLAnalyzer gl;
	public static Analyzer GetByName(string name)
	{
		return name.ToLowerInvariant() switch
		{
			"glfw" or "glfwanalyzer" => glfw ??= new(),
			"opengl" or "glanalyzer" => gl ??= new GLAnalyzer(),
			_ => throw new ArgumentException(name, nameof(name))
		};
	}
}
