using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// The flags on a window.
/// </summary>
[Flags]
public enum WindowFlags : UInt32
{
	/// <summary>
	/// Window is in fullscreen mode.
	/// </summary>
	Fullscreen = 0x00000001,
	/// <summary>
	/// Window usable with OpenGL context.
	/// </summary>
	OpenGL = 0x00000002,
	/* 0x00000004 was SDL_WINDOW_SHOWN in SDL2, please reserve this bit for sdl2-compat. */
	/// <summary>
	/// Window is not visible.
	/// </summary>
	Hidden = 0x00000008,
	/// <summary>
	/// Disables window decoration.
	/// </summary>
	Borderless = 0x00000010,
	/// <summary>
	/// Prevents window resizing.
	/// </summary>
	Resizable = 0x00000020,
	/// <summary>
	/// Window is minimized.
	/// </summary>
	Minimized = 0x00000040,
	/// <summary>
	/// Window is maximized.
	/// </summary>
	Maximized = 0x00000080,
	/// <summary>
	/// Window has grabbed mouse input.
	/// </summary>
	MouseGrabbed = 0x00000100,
	/// <summary>
	/// Window has input focus.
	/// </summary>
	InputFocus = 0x00000200,
	/// <summary>
	/// Window has mouse focus.
	/// </summary>
	MouseFocus = 0x00000400,
	/* 0x00001000 was SDL_WINDOW_FULLSCREEN_DESKTOP in SDL2, please reserve this bit for sdl2-compat. */
	/// <summary>
	/// Window not created by SDL.
	/// </summary>
	Foreign = 0x00000800,
	/* 0x00002000 was SDL_WINDOW_ALLOW_HIGHDPI in SDL2, please reserve this bit for sdl2-compat. */
	/// <summary>
	/// Window has mouse captured (unrelated to <see cref="MouseGrabbed"/>)
	/// </summary>
	MouseCapture = 0x00004000,
	/// <summary>
	/// Window should always be above others.
	/// </summary>
	AlwaysOnTop = 0x00008000,
	/// <summary>
	/// Window should not be added to the taskbar.
	/// </summary>
	SkipTaskbar = 0x00010000,
	/// <summary>
	/// Window should be treated as a utility window.
	/// </summary>
	Utility = 0x00020000,
	/// <summary>
	/// Window should be treated as a tooltip.
	/// </summary>
	Tooltip = 0x00040000,
	/// <summary>
	/// Window should be treated as a popup menu.
	/// </summary>
	PopupMenu = 0x00080000,
	/// <summary>
	/// Window has grabbed keyboard input.
	/// </summary>
	KeyboardGrabbed = 0x00100000,
	/// <summary>
	/// Window usable for Vulkan surface.
	/// </summary>
	Vulkan = 0x10000000,
	/// <summary>
	/// Window usable for Metal view.
	/// </summary>
	Metal = 0x20000000,
	/// <summary>
	/// Window with transparent buffer.
	/// </summary>
	Transparent = 0x40000000,
}