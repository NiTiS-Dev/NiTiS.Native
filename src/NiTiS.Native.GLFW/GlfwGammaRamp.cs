namespace NiTiS.Native.GLFW;

/// <summary>
/// This describes the gamma ramp for a monitor.
/// </summary>
/// <remarks>
/// Added in version 3.0.
/// </remarks>
public unsafe struct GlfwGammaRamp
{
	/// <summary>
	/// An array of value describing the response of the red channel.
	/// </summary>
	public ushort* red;

	/// <summary>
	/// An array of value describing the response of the green channel.
	/// </summary>
	public ushort* green;

	/// <summary>
	/// An array of value describing the response of the blue channel.
	/// </summary>
	public ushort* blue;

	/// <summary>
	/// The number of elements in each array.
	/// </summary>
	public uint size;
}