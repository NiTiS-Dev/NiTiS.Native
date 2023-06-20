using CodeGen.Signature;
using NiTiS.Core.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeGen.Analyzers;

public sealed partial class SDL3Analyzer : Analyzer
{
	[GeneratedRegex(@"extern( )*?DECLSPEC( )*?(?<RETYPE>[\w\d_]+)( )*?SDLCALL( )*?SDL_(?<FUNAME>[\w\d_]+)\((?<ARGS>.*?)\);", RegexOptions.Singleline)]
	private static partial Regex FunctionRegex();

	[GeneratedRegex(@"(([a-zA-Z0-9_]+)\s{0,}(\/\*(\s|\w)*\*\/)?)$")]
	private static partial Regex LastWordRegex();

	private static StaticClassSignature? Sdl;
	public override void Analyze(CodeGenTask task, string content, CompilationSignature unit)
	{
		Sdl = unit.GetTypeOrCreate<StaticClassSignature>(task.Output?.MainClass);
		foreach (Match match in FunctionRegex().Matches(content).Cast<Match>())
		{
			string returnType = match.Groups["RETYPE"].Value;
			string args = match.Groups["ARGS"].Value.Replace('\n', ' ').Replace('\r', ' ');
			string funName = "SDL_" + match.Groups["FUNAME"].Value;

			FunctionSignature sing = new()
			{
				Name = funName,
				NativeName = funName,
				Convention = CallConv.Cdecl,
				ReturnType = GetType(task, returnType),
			};

			string[] argArray = args.Split(',');

			if (argArray.Length == 1 && argArray[0].Trim() is "void")
				goto SKIP_ARGS;

			for (int i = 0; i < argArray.Length; i++)
			{
				string argLine = argArray[i].Trim();
				string varName = LastWordRegex().Match(argLine).Value;
				int varNameLen = varName.Length;
				string argTypeName = argLine.Remove(argLine.Length - varNameLen, varNameLen).Trim();

				//Console.WriteLine($"{argLine} -> {GetType(task, argTypeName).Name}");

				sing.Arguments.Add(new ArgumentSignature()
				{
					NativeName = varName,
					Type = GetType(task, argTypeName)
				});
			}
			SKIP_ARGS:

			Sdl.Functions.Add(sing);
			//Console.WriteLine($"{sing.Name}: {sing.ReturnType}");
		}
	}
	private unsafe BasicTypeSignature GetType(CodeGenTask task, string typeName)
	{
		bool @const = false;

		if (typeName.StartsWith("SDL_OUT_Z_CAP("))
		{
			typeName = typeName["SDL_OUT_Z_CAP(".Length..];
		}

		if (typeName.HasPrefix("const"))
		{
			@const = true;
			typeName = typeName["const".Length..].Trim();

			for (int i = 0; i < typeName.Length; i++)
			{
				if (typeName[i] == ')')
				{
					typeName = typeName[i..];
					break;
				}
			}
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

		typeName = typeName.Trim();

		if (task.Map?.TypeMap?.TryGetValue(typeName, out string? name) ?? false) // typemap check
		{
			return new StaticClassSignature() { Name = name + '*'.Repeat(ptrs) };
		}

		BasicTypeSignature? selectedType = null;

		switch (typeName)
		{
			case "SDL_Window":
				selectedType = new StaticClassSignature() { Name = "SdlWindow" };
				ptrs--;
				break;
			case "int" or "Sint32":
				selectedType = StdTypes.Int;
				break;
			case "void":
				selectedType = StdTypes.Void;
				break;
			case "char" or "wchar_t":
				selectedType = (@const && ptrs > 0) ? StdTypes.CString : StdTypes.SByte;
				if (ptrs > 0)
					ptrs--;
				break;
			case "Uint32":
				selectedType = StdTypes.UInt;
				break;
			case "float":
				selectedType = StdTypes.Float;
				break;
			case "double":
				selectedType = StdTypes.Double;
				break;
			case "bool":
				selectedType = StdTypes.Boolean;
				break;
			case "Sint8":
				selectedType = StdTypes.SByte;
				break;
			case "Uint8":
				selectedType = StdTypes.Byte;
				break;
			case "Sint16":
				selectedType = StdTypes.Short;
				break;
			case "Uint16":
				selectedType = StdTypes.UShort;
				break;
			case "Sint64" or "long":
				selectedType = StdTypes.Long;
				break;
			case "Uint64":
				selectedType = StdTypes.ULong;
				break;
		}

		if (selectedType is null)
			return new StaticClassSignature() { Name = "_" + typeName + "_" + '*'.Repeat(ptrs) };
		else
			return new StaticClassSignature() { Name = selectedType.Name + '*'.Repeat(ptrs) };
	}
}