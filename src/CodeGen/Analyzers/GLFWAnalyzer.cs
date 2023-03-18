using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CodeGen.Signature;
using NiTiS.Core;

namespace CodeGen.Analyzers;

public sealed partial class GLFWAnalyzer : Analyzer
{
	[GeneratedRegex(@"^#define( )+(?<DEF_NAME>\w+)( )+(?<DEF_VAL>.+)$", RegexOptions.Multiline)]
	private static partial Regex DefineRegex();


	public override void Analyze(CodeGenTask task, string content, CompilationSignature sign)
	{
		MatchCollection matches = DefineRegex().Matches(content);

		List<CodeGenRange> appliedRanges = new(4);

		appliedRanges.Add(new() { Name = "GlfwEnum" });
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
			});
		}
	}
}