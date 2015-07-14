using System;
using MonoDevelop.Projects;
using System.Reflection;

namespace Eto.Addin.XamarinStudio
{
	public class VBDotNetProjectBinding : DotNetProjectBinding
	{
		public override string Name
		{
			get { return "Eto.VBDotNet"; }
		}

		protected override DotNetProject CreateProject(string languageName, ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{
			// Support more options when creating a VB.NET project:
			// 1) turn off msbuild since it is not supported for VB.NET projects in Xamarin Studio
			// 2) support adding vb imports
			var project = base.CreateProject(languageName, info, projectOptions);
			var assemblyProject = project as DotNetAssemblyProject;
			if (assemblyProject != null)
			{
				var imports = projectOptions.GetAttribute("VBImports");
				if (imports != null)
				{
					try
					{
						var importType = Type.GetType("MonoDevelop.VBNetBinding.Import, MonoDevelop.VBNetBinding");
						if (importType != null)
						{
							var importReferences = imports.Split(new [] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

							foreach (var elem in importReferences)
							{
								// use reflection to avoid hard ref
								var import = Activator.CreateInstance(importType, elem.Trim());
								assemblyProject.Items.Add((ProjectItem)import);
							}
						}
					}
					catch
					{
						// ignore
					}
				}
				assemblyProject.UseMSBuildEngine = false;
			}
			return project;
		}
	}
}

