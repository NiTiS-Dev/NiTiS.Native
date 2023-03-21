using CodeGen.Signature;
using NiTiS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CodeGen.Analyzers;

public sealed partial class GLFWAnalyzer : Analyzer
{
	[GeneratedRegex(@"^#define( )+(?<DEF_NAME>\w+)( )+(?<DEF_VAL>.+)$", RegexOptions.Multiline)]
	private static partial Regex DefineRegex();

	[GeneratedRegex(@"^GLFWAPI( )*(const)?( )*(?<UNSIGNED>unsigned)?( )*(?<RETURN>[a-zA-Z0-9*&_]*)( )*(?<FUNC_NAME>[a-zA-Z0-9]*)( )*\((?<ARGUMENTS>(((const)?( )*(unsigned)?( )*([a-zA-Z0-9*&_\[\]]*)( )*([a-zA-Z0-9]*)?,)*( )*(const)?( )*(unsigned)?( )*([a-zA-Z0-9*&_\[\]]*)( )*([a-zA-Z0-9]*)?))?\);", RegexOptions.Multiline)]
	private static partial Regex FunctionRegex();

	[GeneratedRegex(@"(?<ARGS>()*(const)?( )*(?<U>unsigned)?()*(?<RETURN>[a-zA-Z0-9*&_]*)( )*,?()*)*")]
	private static partial Regex ArgRegex();
	[GeneratedRegex(@"(\s+([a-zA-Z0-9_]*)\s*)$")]
	private static partial Regex LastWordRegex();

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

			StaticClassSignature glfw = sign.GetTypeOrCreate<StaticClassSignature>(task.Output?.MainClass);

			foreach (Match match in matches)
			{
				string functionName = match.Groups["FUNC_NAME"].Value;
				string returnType = match.Groups["RETURN"].Value;
				bool returnIsUnsigned = match.Groups["UNSIGNED"].Value == "unsigned";
				
				Match args = ArgRegex().Match(match.Groups["ARGUMENTS"].Value);

				FunctionSignature newFunc = new()
				{
					Name = functionName,
					ReturnType = GetType(task, returnType, ref Unsafe.NullRef<string?>(), returnIsUnsigned)
				};


				if (args.Value != "void")
				{
					int argI = 0;
					foreach (string argType in args.Value.Split(','))
					{
						argI++;
						string? argumentName = null;
						BasicTypeSignature argument = GetType(task, argType, ref argumentName);
						Console.WriteLine($">>{newFunc.ReturnType}<< !{functionName}! ({argI}) :{argument.Name}: #{argType}#");
						
						newFunc.Arguments.Add(new ArgumentSignature()
						{
							NativeName = argumentName,
							Type = argument
						});
					}
				}

				glfw.Functions.Add(newFunc);
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

	private unsafe BasicTypeSignature GetType(CodeGenTask task, string typeName, ref string? argumentNameRetusa, bool? unsigned = null)
	{
		typeName = typeName.Trim();

        if (!Unsafe.IsNullRef(ref argumentNameRetusa))
		{ // Remove last word (arg name)
			Match match = LastWordRegex().Match(typeName);
			if (match.Index != -1)
			{
				argumentNameRetusa = typeName.Substring(match.Index, match.Length).Trim();
				typeName = typeName.Remove(match.Index, match.Length);
			}
			else
			{
                Console.WriteLine($"[WRN]: {typeName} has no argument name");
            }
		}

		if (unsigned is null)
		{
			int unsignedIndex = typeName.IndexOf("unsigned");

			if (unsignedIndex != -1)
			{
				typeName = typeName.Substring(unsignedIndex, "unsigned".Length).Trim();
				unsigned = true;
			}
		}
		
		if (typeName.HasPrefix("const"))
		{
			typeName = typeName.Substring("const".Length).Trim();
		}
		
		int ptrs = 0;
		for (int i = 0; i < typeName.Length;)
		{
			char c = typeName[i];
			if (c is '*' or '&' or '[')
			{
				ptrs++;
				typeName = typeName.Remove(i, 1);
			}
			else
			{
				i++;
			}
		}

		return GetType(task, typeName, unsigned.GetValueOrDefault(), ptrs);
	}
	private unsafe BasicTypeSignature GetType(CodeGenTask task, string typeName, bool unsigned, int pointers)
	{
		if (typeName is "void")
		{
			if (pointers is 1)
				return StdTypes.NInt;
			else if (pointers is 0)
				return StdTypes.Void;
		}
		else if (typeName == "size_t")
		{
			return StdTypes.NUInt;
		}

		BasicTypeSignature selectedType = null;

		switch (typeName)
		{
			case "int":
			case "int32_t":
				selectedType = StdTypes.Int;
				break;

			case "uint":
			case "uint32_t":
				selectedType = StdTypes.UInt;
				break;

			case "char":
				if (pointers >= 1)
				{
					selectedType = StdTypes.CString;
					pointers--;
				}
				else
				{
					selectedType = unsigned ? StdTypes.Byte : StdTypes.SByte;
				}
				break;
			case "short":
			case "int16_t":
				selectedType = StdTypes.Short;
				break;
			case "ushort":
			case "uint16_t":
				selectedType = StdTypes.UShort;
				break;

			case "float":
				selectedType = StdTypes.Float;
				break;
			case "double":
				selectedType = StdTypes.Double;
				break;
			case "uint64_t":
				selectedType = StdTypes.ULong;
				break;
			case "int64_t":
				selectedType = StdTypes.Long;
				break;
		}

		if (task.Map?.TypeMap.TryGetValue(typeName, out string? name) ?? false) {
			return new StaticClassSignature() { Name = name + '*'.Repeat(pointers) };
		}

		if (selectedType is null)
			return new StaticClassSignature() { Name = "_" + typeName + "_" + '*'.Repeat(pointers) };
		else
			return new StaticClassSignature() { Name = typeName + '*'.Repeat(pointers) };
	}
}