using CodeGen.Signature;
using System;

namespace CodeGen.Analyzers;

public abstract class Analyzer
{
	public abstract void Analyze(string content, SignatureUnit unit);
}