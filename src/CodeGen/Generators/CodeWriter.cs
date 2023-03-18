﻿using System.IO;
using System.Text;

namespace CodeGen.Builders;

public sealed class CodeWriter : StringWriter
{
	private static readonly string CompilerGenerated = "[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]";
	private static readonly string Tabulatum = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t"; // 32 tabulations
	public int Depth { get; set; } = 0;

	public void BeginBlock()
	{
		WriteLine("{");
		Depth++;
	}
	public void EndBlock()
	{
		Depth--;
		WriteLine("}");
	}
	public void PushCompilerGenerated()
		=> WriteLine(CompilerGenerated);
	public override void WriteLine(string? value)
	{
		WriteIndent();
		base.WriteLine(value);
	}

	public void WriteIndent()
	{
		base.Write(Indent());
	}

	private string Indent()
	{
		if (Depth >= 32)
		{
			return Tabulatum;
		}

		return Tabulatum.Substring(32 - Depth);
	}
}
