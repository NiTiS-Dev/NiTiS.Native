namespace CodeGen.Signature;

public sealed class EnumValueSignature : UnitSignature
{
	public required string FieldName { get; set; }
	public required string Value { get; set; }
}
