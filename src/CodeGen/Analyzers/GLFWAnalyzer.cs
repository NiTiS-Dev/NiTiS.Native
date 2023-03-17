using System;
using System.Text;
using CodeGen.Signature;

namespace CodeGen.Analyzers;

public sealed class GLFWAnalyzer : Analyzer
{
	public override void Analyze(string content, SignatureUnit unit)
	{
		StringBuilder sb = new();

		bool 
			isLicense = false
			;

		foreach (string line in content.Split(new string[] { "\n\r", "\r\n", "\n" }, StringSplitOptions.None))
		{
			#region License hook
			if (line == " *------------------------------------------------------------------------") // The next lines are license
			{
				isLicense = true;
				continue;
            }

			if (isLicense)
			{
				if (line == " *************************************************************************/") // The license is end
				{
					isLicense = false;
					unit.LicenseContent = sb.ToString();
					continue;
				}
				else
				{
					if (line.Length < 3)
						sb.AppendLine();
					else
						sb.AppendLine(line.Substring(3));
				}
			}
			#endregion
		}
	}
}