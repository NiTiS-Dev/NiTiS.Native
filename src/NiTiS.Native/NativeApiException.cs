using System;

namespace NiTiS.Native;

/// <summary>
/// Represents errors that occur during working with native api.
/// </summary>
public class NativeApiException : Exception
{
	/// <summary>
	/// Initialize a <see cref="NativeApiException"/> error instance with specific <paramref name="message"/> and reference to the inner <paramref name="exception"/>.
	/// </summary>
	public NativeApiException(string message, Exception? exception = null) : base(message, exception) { }
}
