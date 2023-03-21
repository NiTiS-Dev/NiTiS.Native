using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NiTiS.Native;

/// <summary>
/// Wrapper for work with <c>char*</c> or <c>wchar_t*</c>
/// </summary>
[DebuggerDisplay($"\"{nameof(ToString)}(),nq\"")]
public unsafe readonly struct CString : IEquatable<CString>, IEnumerable<char>
{
	private readonly byte* pBegin;

	/// <summary>
	/// Constructs C-like string.
	/// </summary>
	/// <param name="pChar">Pointer to the begin.</param>
	public CString(char* pChar)
	{
		this.pBegin = (byte*)pChar;
	}

	/// <summary>
	/// Constructs C-like string.
	/// </summary>
	/// <param name="pChar">Pointer to the begin.</param>
	public CString(byte* pChar)
	{
		this.pBegin = pChar;
	}

	/// <inheritdoc/>
	public IEnumerator<char> GetEnumerator()
		=> new Enumerable(this.pBegin);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">C-like type string.</param>
	public static implicit operator char*(CString self)
		=> (char*)self.pBegin;

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">C-like type string.</param>
	public static implicit operator byte*(CString self)
		=> self.pBegin;

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">C-like type string.</param>
	public static explicit operator nint(CString self)
		=> (nint)(void*)self.pBegin;

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">C-like type string.</param>
	public static explicit operator nuint(CString self)
		=> (nuint)(void*)self.pBegin;

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">C-like type string.</param>
	public static explicit operator void*(CString self)
		=> self.pBegin;

	/// <summary>
	/// Converts 
	/// </summary>
	/// <param name="self">Reference to string buffer.</param>
	public static implicit operator CString(char* self)
		=> new(self);

	/// <summary>
	/// Converts C-like type string reference.
	/// </summary>
	/// <param name="self">Reference to string buffer.</param>
	public static implicit operator CString(byte* self)
		=> new(self);

	/// <summary>
	/// Converts an unmanaged string to a managed.
	/// </summary>
	/// <returns>Managed equivalent to this string.</returns>
	public override readonly string ToString()
		=> ToString(Encoding.UTF8);

	/// <summary>
	/// Converts an unmanaged string to a managed via <paramref name="encoding"/>.
	/// </summary>
	/// <param name="encoding">Raw string encoding.</param>
	/// <returns>Managed equivalent to this string.</returns>
	[DebuggerStepThrough]
	public readonly string ToString(Encoding encoding)
	{
		if (pBegin == null || pBegin[0] == '\0')
			return string.Empty;

		int size = 0;
		for (; true;)
		{
			byte c = pBegin[size];

			if (c == '\0')
				break;

			size++;
		}

		return encoding.GetString(pBegin, size);
	}

	/// <inheritdoc/>
	public bool Equals(CString other)
		=> this.pBegin == other.pBegin;

	private struct Enumerable : IEnumerator<char>
	{
		private readonly char* p;
		private nint index;
		public Enumerable(char* ptr)
		{
			this.p = ptr;
		}
		public Enumerable(byte* ptr)
		{
			this.p = (char*)ptr;
		}
		public char Current => p[index];
		object IEnumerator.Current => this.Current;
		public void Dispose()
		{

		}
		public bool MoveNext()
		{
			if (p != null && p[index++] == '\0')
				return false;

			return true;
		}
		public void Reset()
		{
			index = 0;
		}
	}
}