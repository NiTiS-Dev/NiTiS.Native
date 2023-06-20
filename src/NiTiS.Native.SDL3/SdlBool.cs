using System.Runtime.CompilerServices;

namespace NiTiS.Native.SDL3;

public readonly struct SdlBool
{
	private readonly uint __value;

	public static SdlBool False => default;
	public static SdlBool True => true;

	public static implicit operator bool(SdlBool @bool)
		=> @bool.__value == 1;

	public static implicit operator SdlBool(bool value)
		=> Unsafe.As<bool, SdlBool>(ref value);

	public static explicit operator uint(SdlBool @bool)
		=> @bool.__value;

	public static explicit operator SdlBool(uint value)
		=> Unsafe.As<uint, SdlBool>(ref value);

	/// <inheritdoc/>
	public override string ToString()
		=> this.__value.ToString();
}