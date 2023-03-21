using CodeGen.Signature;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CodeGen.Analyzers;

public sealed partial class GLFWAnalyzer : Analyzer
{
	[GeneratedRegex(@"^#define( )+(?<DEF_NAME>\w+)( )+(?<DEF_VAL>.+)$", RegexOptions.Multiline)]
	private static partial Regex DefineRegex();

	// TODO: Regex is not work properly
	[GeneratedRegex(@"^GLFWAPI( )*(const)?( )*(?<UNSIGNED>unsigned)?( )*(?<RETURN>[a-zA-Z0-9*&_]*)( )*(?<FUNC_NAME>[a-zA-Z0-9]*)( )*\((?<ARGUMENTS>(((const)?( )*(unsigned)?( )*([a-zA-Z0-9*&_\[\]]*)( )*([a-zA-Z0-9]*)?,)*(const)?( )*(unsigned)?( )*([a-zA-Z0-9*&_\[\]]*)( )*([a-zA-Z0-9]*)?))?\);", RegexOptions.Multiline)]
	private static partial Regex FunctionRegex();

	public override void Analyze(CodeGenTask task, string content, CompilationSignature sign)
	{
		#region Enums
		{
			MatchCollection matches = DefineRegex().Matches(content);

			List<CodeGenRange> appliedRanges = new(4)
			{
				new() { Name = "GlfwConstants" }
			};

			for (int i = 0; i < matches.Count; i++)
			{
				Match match = matches[i];

				string defName = match.Groups["DEF_NAME"].Value;
				string defValue = match.Groups["DEF_VAL"].Value;


				if (task.TryGetRangeByBegin(defName, out CodeGenRange? range))
				{
					appliedRanges.Add(range);
				}

				//Console.WriteLine($"#define {defName} = {defValue} ({Strings.FromArray(appliedRanges, start: "", end: "")})");

				// ===================================

				AppendEntryToEnum(task, sign, appliedRanges, (defName, defValue));

				// ===================================

				for (int i2 = 0; i2 < appliedRanges.Count; i2++)
				{
					if (appliedRanges[i2].To == defName)
					{
						appliedRanges.RemoveAt(i2);
					}
				}
			}
		}
		#endregion
		#region Functions
		{
			MatchCollection matches = FunctionRegex().Matches(content);

			StaticClassSignature glfw = sign.GetTypeOrCreate<StaticClassSignature>(task?.Output?.MainClass);

			foreach (Match match in matches)
			{
				string functionName = match.Groups["FUNC_NAME"].Value;
				string returnType = match.Groups["RETURN"].Value;

				glfw.Functions.Add(new()
				{
					Name = functionName,
					ReturnType = null // VOID in future
				});
            }
		}
		#endregion
	}
	private static void AppendEntryToEnum(CodeGenTask task, CompilationSignature sign, List<CodeGenRange> ranges, (string name, string value) entry)
	{
		(string name, string value) = entry;

		name = task.GetMapping(name);

		Span<CodeGenRange> sranges = CollectionsMarshal.AsSpan(ranges);
		for (int i = 0; i < sranges.Length; i++)
		{
			CodeGenRange range = sranges[i];

			EnumSignature @enum = sign.GetTypeOrCreate<EnumSignature>(range.Name);

			@enum.Namespace = task.DefaultNamespace;

			@enum.Entries.Add(new EnumValueSignature()
			{
				FieldName = name,
				Value = value,
				NativeName = name,
			});
		}
	}
}