using System;

namespace NiTiS.Native;

/// <summary>
/// Context for load contextual API.
/// </summary>
public interface INativeContext
{
	/// <summary>
	/// Gets address for contextual call.
	/// </summary>
	/// <param name="procName">Call name.</param>
	public IntPtr GetProcAddress(string procName);
}