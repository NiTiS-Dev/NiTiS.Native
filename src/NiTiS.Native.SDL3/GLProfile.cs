using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// OpenGL profile options.
/// </summary>
public enum GLProfile : UInt32
{
	Core = 0x0001,
	Compatibility = 0x0002,
	ES = 0x0004,
}