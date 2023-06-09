﻿using System;

namespace NiTiS.Native.GLFW;

/// <summary>
/// Monitor pointer wrap.
/// </summary>
public struct GlfwMonitor
{
	/// <summary>
	/// <see cref="GlfwMonitor"/> is wrapper for pointer referencing, calling this constructor cause an exception.
	/// </summary>
	/// <exception cref="InvalidOperationException">Calling constructor is not allowed.</exception>
	public GlfwMonitor()
	{
		throw new InvalidOperationException();
	}
}