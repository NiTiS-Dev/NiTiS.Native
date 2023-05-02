using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

/// <summary>
/// Native library loader for BSD systems.
/// </summary>
public sealed unsafe class BSDLibraryLoader : NativeLibraryLoader
{
	private const string LibAPI = "libdl";
	private const int RtldNow = 0x002;


	[DllImport(LibAPI)]
	private static extern void* dlopen(string path, int flags);
	[DllImport(LibAPI)]
	private static extern void* dlsym(void* pModule, string name);
	[DllImport(LibAPI)]
	private static extern int dlclose(void* module);
	[DllImport(LibAPI)]
	private static extern CString dlerror();

	/// <inheritdoc/>
	public override unsafe NativeFunctionReference GetProcAddress(NativeLibraryReference pLib, string name)
		=> new(dlsym(pLib, name));
	/// <inheritdoc/>
	public override unsafe NativeLibraryReference LoadLibrary(string path)
	{
		void* pFunc = dlopen(path, RtldNow);

		if (pFunc is null)
		{
			pFunc = dlopen(Path.Combine(AlternatePath, path), RtldNow);
		}

		return new(pFunc);
	}
	/// <inheritdoc/>
	public override void UnloadLibrary(NativeLibraryReference handle)
	{
		int ret = dlclose(handle);
		if (ret != 0)
		{
			throw new NativeLibraryUnloadException($"dlclose returns {ret}: {dlerror()}");
		}
	}
}