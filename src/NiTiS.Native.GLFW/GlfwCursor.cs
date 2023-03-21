using System;

namespace NiTiS.Native.GLFW;

/// <summary>
/// Cursor pointer wrap.
/// </summary>
public struct GlfwCursor
{
	/// <summary>
	/// <see cref="GlfwMonitor"/> is wrapper for pointer referencing, calling this constructor cause an exception.
	/// </summary>
	/// <exception cref="InvalidOperationException">Calling constructor is not allowed.</exception>
	public GlfwCursor()
	{
		throw new InvalidOperationException();
	}
}