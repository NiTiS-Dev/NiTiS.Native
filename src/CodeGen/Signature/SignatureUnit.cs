using System.Collections.Generic;

namespace CodeGen.Signature;

public sealed class SignatureUnit
{
	public string? LicenseContent { get; set; }

	public List<SignatureFunction> Functions { get; } = new();
}