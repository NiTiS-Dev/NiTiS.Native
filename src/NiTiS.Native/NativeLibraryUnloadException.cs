using System;

namespace NiTiS.Native;

/// <summary>
/// Represents errors that occur during native library unloading.
/// </summary>
public sealed class NativeLibraryUnloadException : NativeApiException
{
	/// <summary>
	/// Initialize a <see cref="NativeLibraryUnloadException"/> error instance with specific <paramref name="message"/> and reference to the inner <paramref name="exception"/>.
	/// </summary>
	public NativeLibraryUnloadException(string message, Exception? exception = null) : base(message, exception) { }
}