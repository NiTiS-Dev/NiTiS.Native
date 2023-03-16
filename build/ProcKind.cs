namespace NiTiS.Native.NUKE;

public readonly struct ProcKind
{
	private readonly string kind;
    private ProcKind(string kind)
	{
		this.kind = kind;
	}
	public ProcKind()
		=> throw new System.InvalidOperationException();

    public static implicit operator string(ProcKind kind)
		=> kind.kind;

	/// <summary>
	/// Windows runtime.
	/// </summary>
	public static readonly ProcKind WinX32, WinX64, WinARM64;
	/// <summary>
	/// apple osx runtime.
	/// </summary>
	public static readonly ProcKind osxX64, osxARM64;
	public static readonly ProcKind LinuxX64, LinuxARM64;

	static ProcKind()
	{
		WinX32 = new("win-x86");
		WinX64 = new("win-x64");
		WinARM64 = new("win-arm64");

		LinuxX64 = new("linux-x64");
		LinuxARM64 = new("linux-arm64");

		osxX64 = new("osx-x64");
		osxARM64 = new("osx-arm64");
	}
}
