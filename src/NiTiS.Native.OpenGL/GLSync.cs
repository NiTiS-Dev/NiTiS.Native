using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Native.OpenGL;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct GLSync
{
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[FieldOffset(0)]
	public void* __GLsync;
	[FieldOffset(0)]
	public nint Value;

	public GLSync(void * self)
	{
		this.__GLsync = self;
	}

	public static implicit operator nint(GLSync x)
		=> Unsafe.As<GLSync, nint>(ref x);
	public static implicit operator nuint(GLSync x)
		=> Unsafe.As<GLSync, nuint>(ref x);
	public static implicit operator void *(GLSync x)
		=> x.__GLsync;

	public static implicit operator GLSync(nint x)
		=> Unsafe.As<nint, GLSync>(ref x);
	public static implicit operator GLSync(nuint x)
		=> Unsafe.As<nuint, GLSync>(ref x);
	public static implicit operator GLSync(void* x)
		=> new(x);
}