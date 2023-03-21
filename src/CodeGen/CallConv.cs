namespace CodeGen;

public sealed class CallConv
{
	public required string Name { get; init; }
	public bool IsImplicit { get; init; }
	private CallConv()
	{
		
	}

	public static CallConv Default, Cdecl, StdCall, FastCall;
	static CallConv()
	{
		Default = new() { IsImplicit = true, Name = null! };
		Cdecl = new() { Name = "Cdecl" };
		StdCall = new() { Name = "Stdcall" };
		FastCall = new() { Name = "Fastcall" };
	}
}
