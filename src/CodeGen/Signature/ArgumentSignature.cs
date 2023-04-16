namespace CodeGen.Signature;

public sealed class ArgumentSignature : UnitSignature
{
	public string? Comment { get; set; } = null;
	public BasicTypeSignature Type { get; set; }
}