using NiTiS.Core;
using System;

namespace NiTiS.Native.GLFW;

/// <summary>
/// Access 
/// </summary>
public static unsafe partial class Glfw
{
	/// <summary>
	/// Represent <see langword="true"/> value
	/// </summary>
	public const int True = 1;
	public const int False = 0;
	public const int DontCare = -1;

	private static string LibName()
	{
		if (MachineInfo.IsWindows)
			return "glfw3.dll";
		else if (MachineInfo.IsLinux || MachineInfo.IsAndroid)
			return "libglfw.so.3";
		else if (MachineInfo.IsMacos || MachineInfo.IsIos)
			return "libglfw.3.dylib";
		else
			throw new PlatformNotSupportedException();
	}
}