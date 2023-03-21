namespace CodeGen.Signature;

public sealed class FunctionSignature
{
	public string? Name { get; set; }
	public BasicTypeSignature ReturnType { get; set; }
	public CallConv Convention { get; set; } = CallConv.Default;
}