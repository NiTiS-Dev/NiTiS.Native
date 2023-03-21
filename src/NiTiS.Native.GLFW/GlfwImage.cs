namespace NiTiS.Native.GLFW;

/// <summary>
/// This describes a single 2D image.
/// See the documentation for each related
/// function what the expected pixel format is.
/// </summary>
/// <remarks>
/// Added in version 2.1.
/// </remarks>
public unsafe struct GlfwImage
{
	/// <summary>
	/// The width, in pixels, of this image.
	/// </summary>
	public int width;
	/// <summary>
	/// The height, in pixels, of this image.
	/// </summary>
	public int height;
	/// <summary>
	/// The pixel data of this image, arranged left-to-right, top-to-bottom.
	/// </summary>
	public byte* pixels;
}
