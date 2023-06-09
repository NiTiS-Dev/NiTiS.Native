﻿using CodeGen.Builders;
using CodeGen.Signature;
using NiTiS.Collections;
using NiTiS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NiTiS.Core.Extensions;
using System.Text;

namespace CodeGen.Generators;

public sealed class TypeGenerator
{
	private readonly CodeGenTask task;

	public TypeGenerator(CodeGenTask task)
	{
		this.task = task;
	}
	public void PreInitialize(BasicTypeSignature sign)
	{
		if (sign.Name.Contains(':'))
		{
			int index = sign.Name.IndexOf(':');

			string @namespace = sign.Name.Remove(index);
			string name = sign.Name.Substring(index + 1);

			sign.Namespace = @namespace;
			sign.Name = name;
		}
		else
			sign.Name = ConstCaseToPascalCase(sign.Name);

		if (sign is EnumSignature enumSign)
		{

			string?
				postfix = null,
				prefix = null
				;
			{
				CodeGenRange? range = task?.Map?.Ranges?.Where(rng => rng.Name == enumSign.Name)?.FirstOrDefault();
				prefix = range?.Prefix;
				postfix = range?.Postfix;
			}

            for (int enumValueId = 0; enumValueId < enumSign.Entries.Count; enumValueId++)
			{
				EnumValueSignature valSign = enumSign.Entries[enumValueId];

				valSign.FieldName = ConstCaseToPascalCase(valSign.FieldName); // Normalize enum value name

				if (valSign.FieldName.HasPrefix(prefix)) // Remove prefix
				{
					valSign.FieldName = valSign.FieldName[prefix!.Length..];
				}
				if (valSign.FieldName.HasPostfix(postfix)) // Remove postfix
				{
                    valSign.FieldName = valSign.FieldName[..^postfix!.Length];
				}
			}

			List<EnumValueSignature> originOrder = new(enumSign.Entries);

			enumSign.Entries.Sort((cmp, cmp2) => cmp2.FieldName.Length - cmp.FieldName.Length);

			foreach (EnumValueSignature valSign in enumSign.Entries)
			{
                if (task?.Map?.Rename?.TryGetValue(valSign.FieldName, out string? replacement) ?? false) {
                    valSign.FieldName = replacement;
				}

				foreach (EnumValueSignature valSign2 in enumSign.Entries)
				{
					if (valSign2.NativeName is null)
						continue;

					valSign.Value = valSign.Value.Replace(valSign2.NativeName, valSign2.FieldName);
				}

				valSign.Value = valSign.Value.Trim(' ', '\n', '\r');
			}

			enumSign.Entries = originOrder; // Restore right order
		}
		else if (sign is NonEnumSignature typeSign)
		{
			foreach (FunctionSignature fun in typeSign.Functions)
			{

			}
		}
	}
	public void Generate(BasicTypeSignature sign)
	{
		using CodeWriter cw = new();

		string fileName = task?.Output?.TargetDirectory is null
			? Path.Combine(sign.AddictivePath, sign.Name + ".gen.cs")
			: Path.Combine(task.Output.TargetDirectory, sign.AddictivePath, sign.Name + ".gen.cs");

		bool partial = File.Exists(Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, sign.Name + ".cs"));

		cw.Write($"""
			/*  This file is automatic generated by NiTiS-Dev/NiTiS.Native:CodeGen
			 *  Source files: {task?.IncludeFiles?.EnumerableToString()}
			 */
			""");

		cw.WriteLine();
		cw.WriteLine("#pragma warning disable 1591");
		cw.WriteLine();

		if (!string.IsNullOrWhiteSpace(sign.Namespace))
			cw.WriteLine($"namespace {sign.Namespace};");

		cw.WriteLine();
		cw.PushCompilerGenerated();
		cw.WriteLine($"public {(partial ? "partial " : string.Empty)}{sign.TypeKeyword} {sign.Name}");
		cw.BeginBlock();
		{
			if (sign is EnumSignature enumSign)
				GenerateEnum(enumSign, cw);
			else if (sign is NonEnumSignature typeSign)
				GenerateType(typeSign, cw);
		}
		cw.EndBlock();

		cw.WriteLine();
		cw.WriteLine("#pragma warning restore");
		cw.WriteLine();

		string? dirName = Path.GetDirectoryName(fileName);
		if (!Directory.Exists(dirName))
			Directory.CreateDirectory(dirName!);

		File.WriteAllText(fileName, cw.ToString());
	}
	private void GenerateEnum(EnumSignature sign, CodeWriter cw)
	{
		foreach (EnumValueSignature val in sign.Entries)
		{
			cw.WriteLine($"{(char.IsDigit(val.FieldName[0]) ? "_" + val.FieldName: val.FieldName)} =  unchecked(({sign.Parent.Name}){val.Value}),");
		}
	}
	private void GenerateType(NonEnumSignature sign, CodeWriter cw)
	{
		bool functionsExists = false;

		// Delegates
		foreach (FunctionSignature fun in sign.Functions)
		{
			if (task.Map.IgnoreFunctions?.Contains(fun.Name) ?? false)
				continue;

			cw.PushHide();
			cw.PushEditorHide();
			cw.PushCompilerGenerated();
			cw.WriteIndent();
			cw.Write("public ");
			cw.Write(sign.IsStatic ? "static " : string.Empty);
			cw.Write("unsafe readonly delegate* ");
			if (!fun.Convention.IsImplicit)
			{
				cw.Write($"unmanaged[{fun.Convention.Name}] ");
			}

			cw.Write('<');
			foreach (ArgumentSignature arg in fun.Arguments)
			{
				cw.Write($"{arg.Type.Name} /* {arg.NativeName} */, ");
			}
			cw.Write(fun.ReturnType.Name);
			cw.Write('>');

			cw.Write(" @__");
			cw.Write(fun.Name);
			cw.Write(";");

			cw.WriteLine();
			cw.WriteLine();
		}
		// Methods
		foreach (FunctionSignature fun in sign.Functions)
		{
			// Comments
			foreach (ArgumentSignature argSig in fun.Arguments)
			{
				if (argSig.Comment is null)
					continue;

				cw.WriteLine(@$"/// <param name=""{argSig.NativeName}"">{argSig.Comment}</param>");
			}
			cw.PushCompilerGenerated();
			cw.PushInline();
			cw.WriteIndent();
			cw.Write("public ");
			cw.Write(sign.IsStatic ? "static unsafe " : string.Empty);
			cw.Write(fun.ReturnType.Name);
			cw.Write(' ');
			if (fun.Name?.HasPrefix("glfw") ?? false)
				cw.Write(fun.Name[4..]);
			else if (fun.Name?.HasPrefix("gl") ?? false)
				cw.Write(fun.Name[2..]);
			else if (fun.Name?.HasPrefix("wgl") ?? false)
				cw.Write(fun.Name[3..]);
			else if (fun.Name?.HasPrefix("SDL_") ?? false)
				cw.Write(fun.Name[4..]);
			else
				cw.Write(fun?.Name ?? "__UNNAMED__");

			int i = 0;
			cw.Write(fun.Arguments.EnumerableToString((obj) =>
			{
				i++;
				functionsExists = true;
				if (obj is ArgumentSignature argSign)
				{
					string? argName = null;

					argName = argSign.NativeName;
					if (argName is "string")
						argName = "str";

					argName ??= $"arg{i}";

					return $"{argSign.Type.Name} {argName}";
				}

				return "???";
			}, start: "(", end: ")"));
			i = 0;
			cw.WriteLine();
			cw.BeginBlock();
			{
				cw.WriteLine($"{(fun.ReturnType.Name != "void" ? "return" : string.Empty)} @__{fun.Name}{fun.Arguments.EnumerableToString((obj) =>
				{
					i++;
					if (obj is ArgumentSignature argSign)
					{
						if (argSign.NativeName is "string")
							return "str";

						return argSign.NativeName ?? $"arg{i}";
					}
					else
					{
						return $"arg{i}";
					}
				}, start: "(", end: ")")};");
			}
			cw.EndBlock();
		}
		// Static ctor
		if (functionsExists)
		{
			if (task.ContextualApi)
			{
				cw.WriteLine($"static unsafe {sign.Name}()");
				cw.BeginBlock();
				{
					cw.WriteLine($"global::NiTiS.Native.INativeContext? c = global::NiTiS.Native.Loaders.ContextDome.GetContextByName(ContextName);");
					cw.WriteLine("if (c is null) throw new global::NiTiS.Native.ContextNotLoadedException();");

					StringBuilder castName = new();
					foreach (FunctionSignature fun in sign.Functions)
					{
						castName.Append("delegate* <");
						{
							foreach (ArgumentSignature arg in fun.Arguments)
							{
								castName.Append(arg.Type.Name);
								castName.Append(", ");
							}

							castName.Append(fun.ReturnType.Name);
						}
						castName.Append(">");

						cw.WriteLine($"@__{fun.Name} = ({castName})c.GetProcAddress(\"{fun.Name}\");");
						castName.Clear();
					}
				}
				cw.EndBlock();
			}
			else
			{
				cw.WriteLine($"static unsafe {sign.Name}()");
				cw.BeginBlock();
				{
					cw.WriteLine("global::NiTiS.Native.Loaders.NativeLibraryLoader loader;");
					cw.WriteLine("global::NiTiS.Native.NativeLibraryReference lib;");
					cw.WriteLine("loader = global::NiTiS.Native.Loaders.NativeLibraryLoader.DefaultPlatformLoader;");
					cw.WriteLine("string libname = LibName();");
					cw.WriteLine("lib = loader.LoadLibrary(libname);");

					StringBuilder castName = new();
					foreach (FunctionSignature fun in sign.Functions)
					{
						castName.Append("delegate* <");
						{
							foreach (ArgumentSignature arg in fun.Arguments)
							{
								castName.Append(arg.Type.Name);
								castName.Append(", ");
							}

							castName.Append(fun.ReturnType.Name);
						}
						castName.Append(">");

						cw.WriteLine($"@__{fun.Name} = ({castName})loader.GetProcAddress(lib, \"{fun.Name}\");");
						castName.Clear();
					}
				}
				cw.EndBlock();
			}
		}
	}
	private string ConstCaseToPascalCase(string origin)
	{
		if (origin.Any(static c => char.IsLower(c)))
			return origin;

		Span<char> buffer = stackalloc char[origin.Length];

		int index
			= origin.StartsWith("GLFW_") ? 5
			: origin.StartsWith("GL_") ? 3
			: origin.StartsWith("SDL_") ? 4
			: 0
			;

		bool isNewWord = true;
		int bufferIndex = 0;
		for (;index < origin.Length; index++)
		{
			char c = origin[index];

			if (c is '_')
			{
				isNewWord = true;
			}
			else
			{
				buffer[bufferIndex++] = isNewWord ? char.ToUpper(c) : char.ToLower(c);

				isNewWord = false;
			}
		}

		return new(buffer.Slice(0, bufferIndex));
	}
}
