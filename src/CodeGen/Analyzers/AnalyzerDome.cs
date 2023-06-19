using System;

namespace CodeGen.Analyzers;

public static class AnalyzerDome
{
	private static GLFWAnalyzer glfw;
	private static GLAnalyzer gl;
	private static SDL3Analyzer sdl3;
	public static Analyzer GetByName(string name)
	{
		return name.ToLowerInvariant() switch
		{
			"glfw" or "glfwanalyzer" => glfw ??= new(),
			"opengl" or "glanalyzer" => gl ??= new(),
			"sdl3" or "sdl3analyzer" => sdl3 ??= new(),
			_ => throw new ArgumentException(name, nameof(name))
		};
	}
}
