namespace CodeGen;

public sealed class CallConv
{
	private CallConv()
	{
		
	}

	public static CallConv Default, Cdecl, StdCall, FastCall;

	static CallConv()
	{
		Default = new();
		Cdecl = new();
		StdCall = new();
		FastCall = new();
	}
}
