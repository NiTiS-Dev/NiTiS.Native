using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// SDL window handle.
/// </summary>
public unsafe readonly struct SdlWindow
{
	private readonly IntPtr __value;

	/// <summary>
	/// Initializes <see cref="SdlWindow"/> instance. 
	/// </summary>
	/// <param name="value">Handle to window.</param>
	public SdlWindow(IntPtr value)
	{
		__value = value;
	}

	/// <summary>
	/// Initializes <see cref="SdlWindow"/> instance. 
	/// </summary>
	/// <param name="value">Handle to window.</param>
	public SdlWindow(UIntPtr value)
	{
		__value = (nint)(ulong)value;
	}

	/// <summary>
	/// Initializes <see cref="SdlWindow"/> instance. 
	/// </summary>
	/// <param name="value">Handle to window.</param>
	public SdlWindow(void* value)
	{
		__value = (IntPtr)value;
	}

	/// <summary>
	/// Window handle.
	/// </summary>
	public IntPtr Handle
		=> __value;

	/// <inheritdoc/>
	public override string ToString()
	{
		return $"Window {{ Api: SDL, Handle: {(ulong)__value}}}";
	}
}