namespace NiTiS.Native.GLFW;

/// <summary>
/// Gamepad input state.
/// </summary>
/// <remarks>
/// Added in version 3.3.
/// </remarks>
public unsafe struct GlfwGamepadState
{
	/// <summary>
	/// The states of each <see cref="GlfwGamepadButton">gamepad button</see>, <see cref="GlfwAction.Press"/> or <see cref="GlfwAction.Release"/>.
	/// </summary>
	public fixed byte buttons[15];

	/// <summary>
	/// The states of each <see cref="GlfwGamepadAxis">gamepad axis</see>, in the range -1.0 to 1.0 inclusive.
	/// </summary>
	public fixed float axes[6];
}