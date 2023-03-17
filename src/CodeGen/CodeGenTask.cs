using System.Collections.Generic;
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