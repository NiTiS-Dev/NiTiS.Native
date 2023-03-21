using System;

namespace NiTiS.Native.GLFW;

/// <summary>
/// Window pointer wrap.
/// </summary>
public struct GlfwWindow
{
	/// <summary>
	/// <see cref="GlfwWindow"/> is wrapper for pointer referencing, calling this constructor cause an exception.
	/// </summary>
	/// <exception cref="InvalidOperationException">Calling constructor is not allowed.</exception>
	public GlfwWindow()
	{
		throw new InvalidOperationException();
	}
}