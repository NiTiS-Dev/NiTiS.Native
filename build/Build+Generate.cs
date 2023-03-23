using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace NiTiS.Native.NUKE;

partial class Build
{
	Target DeleteBindings => _ => _
		.Executes(() =>
		{
			foreach (Project project in Solution.Projects)
			{
				AbsolutePath path = project.Path.Parent;

				path.GlobFiles("**.gen.cs").ForEach(e =>
				{
					bool existed = File.Exists(e.ToString());
					File.Delete(e.ToString());
					if (File.Exists(e.ToString()) && existed)
						Logger.Warn("File deleted: " + e.ToString());
				});
			}
		});
	Target GenerateBindings => _ => _
		.After(Clean)
		.DependsOn(DeleteBindings)
		.DependsOn(Restore)
		.Executes(() =>
			{
				Project project = Solution.GetProject("CodeGen");
				if (project == default)
				{
					Logger.Error("CodeGen is not found");
					return;
				}

				DotNetRun
				(
					s => s.SetProjectFile(project)
						.EnableDisableParallel()
						.SetConfiguration("Release")
						.SetApplicationArguments(Path.Combine(RootDirectory, "codegen.yml"))
				);
			});
}