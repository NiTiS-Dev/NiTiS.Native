using System;
using System.Runtime.CompilerServices;

namespace NiTiS.Native;

/// <summary>
/// Reference to native function.
/// </summary>
public readonly unsafe struct NativeFunctionReference
{
	/// <summary>
	/// Reference to unmanaged function.
	/// </summary>
	public readonly IntPtr Handle;

	/// <summary>
	/// Creates a native function reference.
	/// </summary>
	/// <param name="pFunction">Pointer to the function.</param>
	public NativeFunctionReference(void* pFunction)
	{
		this.Handle = (IntPtr)pFunction;
	}

	/// <summary>
	/// Creates a native function reference.
	/// </summary>
	/// <param name="function">Pointer to the function.</param>
	public NativeFunctionReference(nint function)
	{
		this.Handle = function;
	}
	
	/// <summary>
	/// Creates a native function reference.
	/// </summary>
	/// <param name="function">Pointer to the function.</param>
	public NativeFunctionReference(nuint function)
	{
		this.Handle = (nint)function;
	}

	/// <inheritdoc/>
	public override string ToString()
		=> Handle.ToString();

	/// <inheritdoc/>
	public override int GetHashCode()
		=> Handle.ToInt32();

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is NativeFunctionReference @ref
		&& @ref.Handle == Handle;

	public static implicit operator void*(NativeFunctionReference self)
		=> self.Handle.ToPointer();

	public static implicit operator NativeFunctionReference(nint @ref)
		=> Unsafe.As<nint, NativeFunctionReference>(ref @ref);

	public static implicit operator NativeFunctionReference(nuint @ref)
		=> Unsafe.As<nuint, NativeFunctionReference>(ref @ref);

	public static implicit operator NativeFunctionReference(void* @ref)
		=> new(@ref);

	public static bool operator ==(NativeFunctionReference left, NativeFunctionReference right)
		=> left.Handle == right.Handle;

	public static bool operator !=(NativeFunctionReference left, NativeFunctionReference right)
		=> left.Handle != right.Handle;
}
