using CodeGen.Signature;
using System;

namespace CodeGen.Analyzers;

public abstract class Analyzer
{
	public abstract void Analyze(CodeGenTask task, string content, CompilationSignature unit);
}