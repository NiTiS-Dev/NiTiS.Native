using System;
using System.Collections.Generic;
using System.Text;

namespace NiTiS.Native.Loaders;

public static class ContextDome
{
	internal static Dictionary<string, INativeContext> contexts = new();
	public static INativeContext? GetContextByName(string contextName)
	{
		if (contextName.Contains(contextName))
			return contexts[contextName];

		return null;
	}

	public static void UploadContext(INativeContext context, string contextName)
	{
		contexts[contextName] = context;
	}
}