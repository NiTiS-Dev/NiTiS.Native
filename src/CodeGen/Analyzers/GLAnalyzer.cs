using CodeGen.Signature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
		#region Enums
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
					if (!groupDict.TryGetValue(group_, out List<(string Name, string Value)>? value))
					{
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

		#endregion Enums

		#region Kinds
		List<Kind> intKinds = new(32);
		foreach (XElement xkinds in root.Elements("kinds"))
		{
			foreach (XElement xkind in root.Elements("kind"))
			{
				string name, desc;
				name = xkind.Attribute("name")?.Value?.ToString()!;
				desc = xkind.Attribute("desc")?.Value?.ToString()!;

				intKinds.Add(new() { Description = desc, Name = name });
			}
		}
		#endregion Kinds

		StaticClassSignature gl = unit.GetTypeOrCreate<StaticClassSignature>(task.Output?.MainClass);

		#region Commands

		foreach (XElement xcommand in root.Element("commands")!.Elements("command"))
		{
			XElement proto = xcommand.Element("proto")!;
			string name = proto.Element("name")!.Value;
			string retusaType;

			{
				int nameIndex = proto.Value.IndexOf(name);
				retusaType = proto.Value.Remove(nameIndex, name.Length);
			}

			FunctionSignature fun = new()
			{
				Name = name,
				ReturnType = new StaticClassSignature() { Name = retusaType },
			};

			gl.Functions.Add(fun);
        }

		#endregion Commands
	}

	public class Kind : IEquatable<Kind>
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;

		public bool Equals(Kind? other)
			=> other is not null ? other.Name == this.Name : false;
		public override bool Equals(object? obj)
			=> Equals(obj as Kind);

		public override int GetHashCode()
			=> Name.GetHashCode();
	}
}
