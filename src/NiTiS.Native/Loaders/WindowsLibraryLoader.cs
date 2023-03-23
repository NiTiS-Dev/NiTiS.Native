using NiTiS.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

/// <summary>
/// Native library loader for Windows systems.
/// </summary>
public sealed unsafe class WindowsLibraryLoader : NativeLibraryLoader
{
	internal override string AlternatePath
		=> ProcessInfo.IsProcess64Bit
		? "runtimes/win-x64/native"
		: "runtimes/win-x86/native"
		;

	[DllImport("kernel32.dll")]
	private static extern void* LoadLibraryA(string path);
	[DllImport("kernel32.dll")]
	private static extern void* GetProcAddress(void* pModule, string name);
	[DllImport("kernel32.dll")]
	private static extern int FreeLibrary(void* module);
	[DllImport("kernel32.dll")]
	private static extern CString GetLastError();

	/// <inheritdoc/>
	public override unsafe NativeFunctionReference GetProcAddress(NativeLibraryReference handle, string methodName)
		=> new(GetProcAddress(handle, methodName));
	/// <inheritdoc/>
	public override unsafe NativeLibraryReference LoadLibrary(string path)
	{
		void* pFunc = LoadLibraryA(path);

		if (pFunc is null)
		{
			pFunc = LoadLibraryA(Path.Combine(AlternatePath, path));
		}

		return new(pFunc);
	}
	/// <inheritdoc/>
	public override void UnloadLibrary(NativeLibraryReference handle)
	{
		int ret = FreeLibrary(handle);
		if (ret != 0)
		{
			throw new NativeLibraryUnloadException($"FreeLibrary returns {ret}", new Win32Exception(ret));
		}
	}
}