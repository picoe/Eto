using System;
using MonoDevelop.Projects;
using System.Linq;
using MonoDevelop.Projects.Extensions;

namespace Eto.Addin.MonoDevelop
{
	#if false
	public class FSharpProjectBinding : DotNetProjectExtension
	{
		public override string Name
		{
			get { return "Eto.FSharp"; }
		}

		protected override DotNetProject CreateProject(string languageName, ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{
			var project = base.CreateProject(languageName, info, projectOptions);
			// fix NRE when creating projects - fixed in monodevelop/master but not applied as of XS 5.9.4
			foreach (var config in project.Configurations.OfType<DotNetProjectConfiguration>())
			{
				var fsharpParameters = config.CompilationParameters;
				if (fsharpParameters != null)
				{
					// use reflection so we don't take a hard dep on MonoDevelop.FSharpBinding when not present (e.g. linux)
					var defineConstantsParameter = fsharpParameters.GetType().GetProperty("DefineConstants");
					if (defineConstantsParameter != null)
					{
						var val = defineConstantsParameter.GetValue(fsharpParameters);
						if (val == null)
							defineConstantsParameter.SetValue(fsharpParameters, string.Empty);
					}
				}
			}
			return project;
		}
	}
	#endif
}

