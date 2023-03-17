namespace CodeGen.Signature;

public sealed class SignatureFunction
{
	public string? Name { get; set; }
	public CallConv Convention { get; set; } = CallConv.Default;
}