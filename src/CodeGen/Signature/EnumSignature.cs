﻿using System.Collections.Generic;

namespace CodeGen.Signature;

public sealed class EnumSignature : BasicTypeSignature
{
	private BasicTypeSignature? enumSize;

    public void SetEnumSizeType(BasicTypeSignature? enumSize)
	{
		this.enumSize = enumSize;
	}
	public override BasicTypeSignature Parent => enumSize ?? StdTypes.Int;
	public override bool IsStatic => false;
	public override string TypeKeyword => "enum";
	public bool IsFlags { get; set; } = false;
	public List<EnumValueSignature> Entries { get; set; } = new();
}
