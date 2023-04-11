using CodeGen.Signature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CodeGen.Analyzers;

public partial class GLAnalyzer : Analyzer
{
	public override void Analyze(CodeGenTask task, string content, CompilationSignature unit)
	{
		XDocument xml;
		{
			StringReader reader = new(content);
			xml = XDocument.Load(reader);
		} // removes reader from method scope

		XElement? root = xml.Element("registry");

		if (root is null)
			throw new Exception("ROOT NOT FOUND!");

		// group, (name, value)
		Dictionary<string, List<(string Name, string Value)>> groupDict = new(512);
		foreach (XElement xenums in root.Elements("enums"))
		{
			string? globalGroup = xenums.Attribute("group")?.Value?.ToString();
			foreach (XElement xenum in xenums.Elements("enum"))
			{
				string val = xenum.Attribute("value")?.Value?.ToString() ?? throw new Exception("Enum has no value!");
				string name = xenum.Attribute("name")?.Value?.ToString() ?? throw new Exception("Enum has no name!");
				
				string? group = xenum.Attribute("group")?.Value?.ToString() ?? globalGroup;

				if (group is null)
				{
					//Console.WriteLine($"Skip: {name}={val};");
					continue;
				}

				foreach (string group_ in group.Split(','))
				{
					if (!groupDict.TryGetValue(group_, out List<(string Name, string Value)>? value)) {
						value = new List<(string Name, string Value)>(64);
						groupDict[group_] = value;
					}

					value.Add((name, val));
				}
			}
		}

		foreach ((string key, List<(string Name, string Value)> values) in groupDict)
		{
			EnumSignature enumSing = new()
			{
				Namespace = task.DefaultNamespace + ".Enums" ?? string.Empty,
				Entries = values.Select(s => new EnumValueSignature() { FieldName = s.Name, Value = s.Value }).ToList(),
				Name = key,
				AddictivePath = "Enums"
			};

			unit.Types.Add(enumSing);
		}
	}
}
