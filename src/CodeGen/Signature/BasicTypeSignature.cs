namespace CodeGen.Signature;

public abstract class BasicTypeSignature : UnitSignature
{
	private static int __index__ = 0;
	public string? Namespace { get; set; }
	public abstract BasicTypeSignature? Parent { get; }
	public string Name { get; set; } = $"__UNNAMED{++__index__}__";
	public abstract bool IsStatic { get; }
	public abstract string TypeKeyword { get; }

	public override string ToString()
		=> Name;
}

public sealed class StructSignature : NonEnumSignature
{
	public override BasicTypeSignature? Parent => null;
	public override bool IsStatic => false;
	public override string TypeKeyword => "struct";
}
public sealed class StaticClassSignature : NonEnumSignature
{
	public override BasicTypeSignature? Parent => null;
	public override bool IsStatic => true;
	public override string TypeKeyword => "class";
}