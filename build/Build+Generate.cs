using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
	Target GenerateBindings => _ => _
		.After(Clean)
		.DependsOn(Restore)
		.Executes(() =>
			{
				var project = Solution.GetProject("CodeGen");
				if (project == default)
				{
					Logger.Error("CodeGen is not found");
					return;
				}

				DotNetRun
				(
					s => s.SetProjectFile(project)
						.SetConfiguration("Release")
						.SetApplicationArguments(Path.Combine(RootDirectory, "codegen.yml"))
				);
			});
}