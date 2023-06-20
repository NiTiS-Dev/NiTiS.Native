using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace CodeGen;


[YamlSerializable]
public sealed class CodeGenTask
{

	[YamlMember(Alias = "base-namespace")]
	public string? DefaultNamespace { get; set; }
	[YamlMember(Alias = "include")]
	public List<string>? IncludeFiles { get; set; }
	[YamlMember(Alias = "output")]
	public CodeGenOutput? Output { get; set; }
	[YamlMember(Alias = "analyzer")]
	public CodeGenAnalyzer? Analyzer { get; set; }
	[YamlMember(Alias = "map")]
	public CodeGenMap? Map { get; set; }
	[YamlMember(Alias = "contextual-api")]
	public bool ContextualApi { get; set; } = false;

	public string GetMapping(string? name)
	{
		if (name is null)
			return string.Empty;

		if (Map is not null && Map.Rename is not null)
		{
			if (Map.Rename.TryGetValue(name, out string? retusa)) {
				return retusa;
			}
		}

		return name;
	}

	public bool TryGetRangeByBegin(string? name, [NotNullWhen(true)] out CodeGenRange? range)
	{
		range = null;
		if (Map?.Ranges is not null)
		{
			for (int i = 0; i < Map.Ranges.Length; i++)
			{
				if (Map.Ranges[i].From == name)
				{
					range = Map.Ranges[i];
					return true;
				}
			}
		}

		return false;
	}
}

[YamlSerializable]
public sealed class CodeGenOutput
{

	[YamlMember(Alias = "target-directory")]
	public string? TargetDirectory { get; set; }
	[YamlMember(Alias = "license")]
	public string? License { get; set; }
	[YamlMember(Alias = "main-class")]
	public string? MainClass { get; set; }
}

[YamlSerializable]
public sealed class CodeGenAnalyzer
{
	[YamlMember(Alias = "name")]
	public string? Name { get; set; }
}

[YamlSerializable]
public sealed class CodeGenMap
{
	[YamlMember(Alias = "ignore")]
	public string[] IgnoreFunctions { get; set; }
	[YamlMember(Alias = "typemap")]
	public Dictionary<string, string>? TypeMap { get; set; }
	[YamlMember(Alias = "rename")]
	public Dictionary<string, string>? Rename { get; set; }
	[YamlMember(Alias = "ranges")]
	public CodeGenRange[] Ranges { get; set; } = Array.Empty<CodeGenRange>();
}

[YamlSerializable]
public sealed class CodeGenRange
{
	[YamlMember(Alias = "from")]
	public string? From { get; set; }
	
	[YamlMember(Alias = "to")]
	public string? To { get; set; }

	[YamlMember(Alias = "name")]
	public string? Name { get; set; }

	[YamlMember(Alias = "prefix")]
	public string? Prefix { get; set; }

	[YamlMember(Alias = "postfix")]
	public string? Postfix { get; set; }

	public override string ToString()
		=> Name!;
}