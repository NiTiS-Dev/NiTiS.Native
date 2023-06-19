using System;

namespace NiTiS.Native.SDL3.Context;

/// <summary>
/// GL context handle.
/// </summary>
public unsafe readonly struct SdlGlContext
{
	private readonly IntPtr __value;

	/// <summary>
	/// Initializes <see cref="SdlGlContext"/> instance.
	/// </summary>
	/// <param name="value">Handle to context.</param>
	public SdlGlContext(IntPtr value)
	{
		__value = value;
	}

	/// <summary>
	/// Initializes <see cref="SdlGlContext"/> instance.
	/// </summary>
	/// <param name="value">Handle to context.</param>
	public SdlGlContext(UIntPtr value)
	{
		__value = (nint)(ulong)value;
	}

	/// <summary>
	/// Initializes <see cref="SdlGlContext"/> instance.
	/// </summary>
	/// <param name="value">Handle to context.</param>
	public SdlGlContext(void* value)
	{
		__value = (IntPtr)value;
	}

	/// <summary>
	/// Context handle.
	/// </summary>
	public IntPtr Handle
		=> __value;

	/// <inheritdoc/>
	public override string ToString()
	{
		return $"Context {{ Api: SDL/OpenGL, Handle: {(ulong)__value}}}";
	}
}