using System.Collections.Generic;

namespace CodeGen.Signature;

public abstract class NonEnumSignature : BasicTypeSignature
{
	public List<FunctionSignature> Functions { get; set; } = new();
}
