namespace NiTiS.Native.GLFW;

public struct GlfwVideoMode
{
	/// <summary>
	/// The width, in screen coordinates, of the video mode.
	/// </summary>
	public int width;
	/// <summary>
	/// The height, in screen coordinates, of the video mode.
	/// </summary>
	public int height;
	/// <summary>
	///  The bit depth of the red channel of the video mode.
	/// </summary>
	public int redBits;
	/// <summary>
	/// The bit depth of the green channel of the video mode.
	/// </summary>
	public int greenBits;
	/// <summary>
	/// The bit depth of the blue channel of the video mode.
	/// </summary>
	public int blueBits;
	/// <summary>
	/// The refresh rate, in Hz, of the video mode.
	/// </summary>
	public int refreshRate;
}