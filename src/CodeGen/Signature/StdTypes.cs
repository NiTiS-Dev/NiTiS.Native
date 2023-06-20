using System.Runtime.InteropServices;

namespace CodeGen.Signature;

public static class StdTypes
{
	public static readonly NonEnumSignature
		Object,
		String,
		CString,
		Int,
		UInt,
		Byte,
		SByte,
		Short,
		UShort,
		Long,
		ULong,
		NInt,
		NUInt,
		Float,
		Double,
		Void,
		Boolean
		;


	static StdTypes()
	{
		Object = new StdTypeSignature()
		{
			Name = "object",
			FullName = "System.Object",
		};
		String = new StdTypeSignature()
		{
			Name = "string",
			FullName = "System.String",
		};
		CString = new StdTypeSignature()
		{
			Name = "CString",
			FullName = "NiTiS.Native.CString"
		};
		Byte = new StdTypeSignature()
		{
			Name = "byte",
			FullName = "System.Byte"
		};
		SByte = new StdTypeSignature()
		{
			Name = "sbyte",
			FullName = "System.SByte"
		};
		Short = new StdTypeSignature()
		{
			Name = "short",
			FullName = "System.Int16"
		};
		UShort = new StdTypeSignature()
		{
			Name = "ushort",
			FullName = "System.UInt16"
		};
		Int = new StdTypeSignature()
		{
			Name = "int",
			FullName = "System.Int32"
		};
		UInt = new StdTypeSignature()
		{
			Name = "uint",
			FullName = "System.UInt32"
		};
		Long = new StdTypeSignature()
		{
			Name = "long",
			FullName = "System.Int64"
		};
		ULong = new StdTypeSignature()
		{
			Name = "ulong",
			FullName = "System.UInt64"
		};
		NInt = new StdTypeSignature()
		{
			Name = "nint",
			FullName = "System.IntPtr"
		};
		NUInt = new StdTypeSignature()
		{
			Name = "nuint",
			FullName = "System.UIntPtr"
		};
		Void = new StdTypeSignature()
		{
			Name = "void",
			FullName = "System.Void"
		};
		Float = new StdTypeSignature()
		{
			Name = "float",
			FullName = "System.Single"
		};
		Double = new StdTypeSignature()
		{
			Name = "double",
			FullName = "System.Double"
		};
		Boolean = new StdTypeSignature()
		{
			Name = "bool",
			FullName = "System.Boolean"
		};
	}
	private sealed class StdTypeSignature : NonEnumSignature
	{
		public bool IsStruct { get; set; } = false;
		public override BasicTypeSignature? Parent => Object;
		public override bool IsStatic => false;
		public override string TypeKeyword => IsStruct ? "struct" : "sealed class";
	}
}