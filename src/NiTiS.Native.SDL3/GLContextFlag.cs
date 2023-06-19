using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// OpenGL context options.
/// </summary>
[Flags]
public enum GLContextFlag : UInt32
{
	Debug = 0x0001,
	ForwardCompatibility = 0x0002,
	RobustAccess = 0x0004,
	ResetIsolation = 0x0008,
}