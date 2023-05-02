#if NET6_0_OR_GREATER
using System;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace NiTiS.Native.Loaders;

/// <summary>
/// .NET native library loader.
/// </summary>
internal class NetLibraryLoader : NativeLibraryLoader
{
	public override NativeFunctionReference GetProcAddress(NativeLibraryReference handle, string methodName)
	{
		nint retusa = default;
		try
		{
			if (NativeLibrary.TryGetExport(handle.Handle, methodName, out retusa))
			{
				goto RETURN;
			}
		}
		catch (ArgumentNullException) { }

	RETURN:
		return retusa;
	}
	public override NativeLibraryReference LoadLibrary(string path)
	{
		nint retusa = default;
		try
		{
			if (NativeLibrary.TryLoad(path, out retusa))
			{
				goto RETURN;
			}
			else if (retusa == IntPtr.Zero &&
				NativeLibrary.TryLoad(Path.Combine(AlternatePath, path), out retusa))
			{
				goto RETURN;
			}
		}
		catch (ArgumentNullException) { }

	RETURN:
		return retusa;
	}

	public override void UnloadLibrary(NativeLibraryReference handle)
	{
		NativeLibrary.Free(handle.Handle);
	}
}
#endif