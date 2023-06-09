﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;

namespace CodeGen;

internal class Program
{
	static int Main(string[] args)
	{
		Console.WriteLine("NiTiS-Dev::CodeGen");
		Console.WriteLine();
		Console.WriteLine();

		string? path = null;

		if (args.Length == 0)
		{
			return -1001;
		}
		else if (args.Length == 1)
		{
			path = args[0];

			if (!File.Exists(path))
			{
				return -1002;
			}
		}
		else
		{
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];

				if (arg.StartsWith("-"))
					continue;

				path = args[i];
			}

			if (!File.Exists(path))
			{
				return -1002;
			}
		}

		var deser = new DeserializerBuilder()
			.Build();

		Dictionary<string, CodeGenTask> tasks = deser.Deserialize<Dictionary<string, CodeGenTask>>(File.ReadAllText(path));

		Generator.Run(tasks);





		return 0;
	}
}
