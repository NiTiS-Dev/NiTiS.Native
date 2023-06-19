using NiTiS.Core.InteropServices;
using NiTiS.Native.SDL3.Context;
using System;
using System.Text;

namespace NiTiS.Native.SDL3;

public static unsafe partial class Sdl3
{
	public static INativeContext CreateContext(SdlWindow window, SdlGlContext context)
	{
		int error = __SDL_GL_MakeCurrent(window, context);

		if (error != 0)
		{
			throw new Exception(__SDL_GetError().ToString());
		}

		delegate* unmanaged[Cdecl]<CString, nint> getProc = null;

		getProc = __SDL_GL_GetProcAddress;

		return new nativeContext(getProc);
	}
	private sealed class nativeContext : INativeContext
	{
		public readonly delegate* unmanaged[Cdecl]<CString, nint> getProcAddress;
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
		public nativeContext(delegate* unmanaged[Cdecl]<CString, nint> getProcAddress)
		{
			this.getProcAddress = getProcAddress;
		}
	}
}