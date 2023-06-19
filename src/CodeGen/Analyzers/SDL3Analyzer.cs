using CodeGen.Signature;
using System;
using System.Text.RegularExpressions;

namespace CodeGen.Analyzers;

public sealed partial class SDL3Analyzer : Analyzer
{
	[GeneratedRegex(@"extern( )*?DECLSPEC( )*?(?<RETYPE>[\w\d]+)( )*?SDLCALL( )*?SDL_(?<FUNAME>[\w\d]+)\((?<ARGS>.*?)\);", RegexOptions.Singleline)]
	private static partial Regex FunctionRegex();

	public override void Analyze(CodeGenTask task, string content, CompilationSignature unit)
	{
		foreach (Match match in FunctionRegex().Matches(content))
		{
			string returnType = match.Groups["RETYPE"].Value;
			string args = match.Groups["ARGS"].Value.Replace('\n', ' ').Replace('\r', ' ');
			string funName = "SDL_" + match.Groups["FUNAME"].Value;

			Console.WriteLine($"{funName} ({args}) : {returnType}");
		}
	}
}