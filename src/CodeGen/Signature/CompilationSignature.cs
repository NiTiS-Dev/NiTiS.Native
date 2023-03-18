using System.Collections.Generic;
using System.Linq;

namespace CodeGen.Signature;

public sealed class CompilationSignature
{
	public string? LicenseContent { get; set; }

	public List<BasicTypeSignature> Types { get; } = new();

	public Type GetTypeOrCreate<Type>(string? name)
		where Type : BasicTypeSignature, new()
	{
		Type? retusa = null;

		retusa = Types.Where(t => t.Name == name).OfType<Type>().FirstOrDefault();

		if (retusa is null)
		{
			retusa = new();

			if (name is not null)
				retusa.Name = name;

			Types.Add(retusa);
			return retusa;
		}

		return retusa;
	}
}