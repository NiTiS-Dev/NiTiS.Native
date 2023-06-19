using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// Window flash operation.
/// </summary>
public enum FlashOperation : UInt32
{
	/// <summary>
	/// Cancel any window flash state.
	/// </summary>
	Cancel,
	/// <summary>
	/// Flash the window briefly to get attention.
	/// </summary>
	Briefly,
	/// <summary>
	/// Flash the window until it gets focus.
	/// </summary>
	UntilFocused,
}