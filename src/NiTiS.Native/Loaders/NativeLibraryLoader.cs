using NiTiS.Core;
using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

/// <summary>
/// Wrapper for working with native libraries.
/// </summary>
public abstract unsafe class NativeLibraryLoader
{
	/// <summary>
	/// Loads library.
	/// </summary>
	/// <param name="path">Path to library file.</param>
	/// <returns>Handle to loaded native library.</returns>
	public abstract NativeLibraryReference LoadLibrary(string path);

	/// <summary>
	/// Unloads library.
	/// </summary>
	/// <param name="handle">Handle to loaded native library.</param>
	public abstract void UnloadLibrary(NativeLibraryReference handle);

	/// <summary>
	/// Returns function with specific name.
	/// </summary>
	/// <param name="handle">Native library.</param>
	/// <param name="methodName">Method name.</param>
	/// <returns>Native method reference.</returns>
	public abstract NativeFunctionReference GetProcAddress(NativeLibraryReference handle, string methodName);

	/// <summary>
	/// Stores native method reference to <paramref name="dst"/>.
	/// </summary>
	/// <param name="dst">Destination for method reference.</param>
	/// <param name="handle">Native library.</param>
	/// <param name="methodName">Method name.</param>
	public void GetProcAddress(ref void* dst, NativeLibraryReference handle, string methodName)
	{
		dst = (void*)GetProcAddress(handle, methodName).Handle;
	}
	internal virtual string? AlternatePath
		=> MachineInfo.IsWindows
			? MachineInfo.Architecture switch
			{
				Architecture.X64 => "runtimes/win-x64/native",
				Architecture.X86 => "runtimes/win-x86/native",
				Architecture.Arm => "runtimes/win-arm/native",
				Architecture.Arm64 => "runtimes/win-arm64/native",
				_ => null
			}
			: MachineInfo.IsLinux
			? MachineInfo.Architecture switch
			{
				Architecture.X64 => "runtimes/linux-x64/native",
				Architecture.X86 => "runtimes/linux-x86/native",
				Architecture.Arm => "runtimes/linux-arm/native",
				Architecture.Arm64 => "runtimes/linux-arm64/native",
				_ => null
			}
			: MachineInfo.IsMacos
			? MachineInfo.Architecture switch
			{
				Architecture.X64 => "runtimes/osx-x64/native",
				Architecture.Arm64 => "runtimes/osx-arm64/native",
				Architecture.Arm => "runtimes/osx-arm/native",
				Architecture.X86 => "runtimes/osx-x86/native",
			}
			: throw new NotImplementedException()
			;

	private static NativeLibraryLoader? loader;

	/// <summary>
	/// Default platform loader.
	/// </summary>
	public static NativeLibraryLoader DefaultPlatformLoader
	{
		get => loader ??= CreateDefaultPlatformLoader()!;
		set => loader = value;
	}

	/// <summary>
	/// Creates library loader for current platform.
	/// </summary>
	/// <returns><see langword="null"/> when platform is not supported.</returns>
	public static NativeLibraryLoader? CreateDefaultPlatformLoader()
	{
#if NET6_0_OR_GREATER
		return new NetLibraryLoader();
#endif

		if (MachineInfo.IsWindows)
			return new WindowsLibraryLoader();
		else if (MachineInfo.IsLinux)
			return new UnixLibraryLoader();
		else if (MachineInfo.IsMacos || RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
			return new BSDLibraryLoader();

		return null;
	}
}