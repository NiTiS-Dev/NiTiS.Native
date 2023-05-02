#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

/// <summary>
/// .NET native library loader.
/// </summary>
internal class NetLibraryLoader : NativeLibraryLoader
{
	public override NativeFunctionReference GetProcAddress(NativeLibraryReference handle, string methodName)
	{
		if (NativeLibrary.TryGetExport(handle.Handle, methodName, out nint retusa))
		{
			return new(retusa);
		}

		return default;
	}

	public override NativeLibraryReference LoadLibrary(string path)
	{
		if (NativeLibrary.TryLoad(path, out nint retusa))
		{
			return new(retusa);
		}

		return default;
	}

	public override void UnloadLibrary(NativeLibraryReference handle)
	{
		NativeLibrary.Free(handle.Handle);
	}
}
#endif