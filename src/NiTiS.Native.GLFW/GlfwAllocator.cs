namespace NiTiS.Native.GLFW;

public unsafe struct GlfwAllocator
{
	public delegate* <nuint /* size */, nint /* user */, nint> allocate;
	public delegate*<nint /*block */, nuint /* size */, nint /* user */, nint> reallocate;
	public delegate*<nint /* block */, nint /* user */, void> deallocate;
	public nint user;
}
