using System.Runtime.CompilerServices;

namespace NiTiS.Native.SDL3;

public readonly struct SdlJoystickId
{
	private readonly uint __value;

	public static SdlJoystickId Nil => default;

	public static implicit operator uint(SdlJoystickId id)
		=> id.__value;

	public static implicit operator SdlJoystickId(uint value)
		=> Unsafe.As<uint, SdlJoystickId>(ref value);

	public static explicit operator int(SdlJoystickId id)
		=> (int)id.__value;

	public static explicit operator SdlJoystickId(int value)
		=> Unsafe.As<int, SdlJoystickId>(ref value);

	/// <inheritdoc/>
	public override string ToString()
		=> $"Joystick {{ Api: SDL, Id: {(ulong)__value}}}";
}