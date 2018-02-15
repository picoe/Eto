using System;
using MonoDevelop.Projects;
using System.Reflection;
using System.Collections.Generic;

namespace Eto.Addin.MonoDevelop
{
	public class VBDotNetProjectExtension : DotNetProjectExtension
	{
		protected override void OnGetTypeTags (HashSet<string> types)
		{
			base.OnGetTypeTags (types);
			types.Add ("Eto.VBNet");
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Project.UseMSBuildEngine = true;

		}
		protected override void OnInitializeFromTemplate (ProjectCreateInformation projectCreateInfo, System.Xml.XmlElement template)
		{
			base.OnInitializeFromTemplate (projectCreateInfo, template);

			// Support more options when creating a VB.NET project:
			// 1) support adding vb imports
			var assemblyProject = Project;
			if (assemblyProject != null) {
				var imports = template.GetAttribute ("VBImports");
				if (imports != null) {
					try {
						var importType = Type.GetType ("MonoDevelop.VBNetBinding.Import, MonoDevelop.VBNetBinding");
						if (importType != null) {
							var importReferences = imports.Split (new [] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

							foreach (var elem in importReferences) {
								// use reflection to avoid hard ref
								var import = Activator.CreateInstance (importType, elem.Trim ());
								assemblyProject.Items.Add ((ProjectItem)import);
							}
						}
					} catch {
						// ignore
					}
				}
			}
		}

		protected override void OnGetDefaultImports (List<string> imports)
		{
			base.OnGetDefaultImports (imports);

			const string vbimport = "$(MSBuildBinPath)\\Microsoft.VisualBasic.targets";

			if (!imports.Contains(vbimport))
				imports.Add (vbimport);
		}
	}
}

