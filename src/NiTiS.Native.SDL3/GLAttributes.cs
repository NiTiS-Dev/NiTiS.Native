using System;

namespace NiTiS.Native.SDL3;

/// <summary>
/// OpenGL configuration attributes.
/// </summary>
public enum GLAttributes : UInt32
{
	RedSize,
	GreenSize,
	BlueSize,
	AlphaSize,
	BufferSize,
	DoubleBuffer,
	DepthSize,
	StencilSize,
	AccumRedSize,
	AccumGreenSize,
	AccumBlueSize,
	AccumAlphaSize,
	Stereo,
	MultisampleBuffers,
	MultisampleSamples,
	AcceleratedVisual,
	RetainedBacking,
	ContextMajorVersion,
	ContextMinorVersion,
	ContextFlags,
	ContextProfileMask,
	ShareWithCurrentContext,
	FramebufferSRGBCapable,
	ContextReleaseBehaviour,
	ContextResetNotification,
	ContextNoError,
	FloatBuffers,
	EGLPlatform
}