using NiTiS.Core;
using NiTiS.Core.InteropServices;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NiTiS.Native.GLFW;

/// <summary>
/// Access 
/// </summary>
public static unsafe partial class Glfw
{
	/// <summary>
	/// Represent <see langword="true"/> value.
	/// </summary>
	public const int True = 1;

	/// <summary>
	/// Represent <see langword="false"/> value
	/// </summary>
	public const int False = 0;
	public const int DontCare = -1;

	/// <summary>
	/// Create context for contextual API of current <paramref name="window"/>.
	/// </summary>
	/// <param name="window">Window for get context.</param>
	public static INativeContext CreateContext(GlfwWindow* window)
	{
		__glfwMakeContextCurrent(window);
		var getProc = __glfwGetProcAddress;

		return new nativeContext(getProc);
	}

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

	private sealed class nativeContext : INativeContext
	{
		public readonly delegate* <CString, nint> getProcAddress;
		IntPtr INativeContext.GetProcAddress(string procName)
		{
			int possibleBytesLen = Encoding.UTF8.GetMaxByteCount(procName.Length) + 1;
			Span<byte> name = stackalloc byte[possibleBytesLen];

			byte* pName = SpanMarshal.GetPointer(name);
			fixed (char* c = procName)
			{
				pName[Encoding.UTF8.GetBytes(c, procName.Length, pName, possibleBytesLen)] = 0;
			}

			return getProcAddress(pName);
		}
        public nativeContext(delegate*<CString, nint> getProcAddress)
        {
            this.getProcAddress = getProcAddress;
        }
    }
}