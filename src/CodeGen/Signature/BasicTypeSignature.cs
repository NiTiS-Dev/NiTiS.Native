using System.Collections.Generic;

namespace CodeGen.Signature;

public abstract class BasicTypeSignature : UnitSignature
{
	private static int __index__ = 0;
	public string? Namespace { get; set; }
	public abstract BasicTypeSignature? Parent { get; }
	public string Name { get; set; } = $"__UNNAMED{++__index__}__";
	public abstract bool IsStatic { get; }
	public abstract string TypeKeyword { get; }
}

public sealed class EnumSignature : BasicTypeSignature
{
	public override BasicTypeSignature? Parent => null;
	public override bool IsStatic => false;
	public override string TypeKeyword => "enum";
	public List<EnumValueSignature> Entries { get; } = new();
}

public sealed class StructSignature : BasicTypeSignature
{
	public override BasicTypeSignature? Parent => throw new System.NotImplementedException();
	public override bool IsStatic { get; }
	public override string TypeKeyword => "struct";
}