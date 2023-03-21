using NiTiS.Core.Extensions;
using System.Collections.Generic;

namespace CodeGen.Signature;

public sealed class FunctionSignature
{
	public string? Name { get; set; }
	public BasicTypeSignature ReturnType { get; set; } = StdTypes.Void;
	public List<BasicTypeSignature> Arguments { get; set; } = new(4);
	public CallConv Convention { get; set; } = CallConv.Default;

	public override string ToString()
		=> ReturnType + Name + Arguments.EnumerableToString(start: "(", end: ")");
}