using System;
using MonoDevelop.Projects;
using System.Linq;
using MonoDevelop.FSharp;

namespace Eto.Addin.XamarinStudio
{
	public class FSharpProjectBinding : DotNetProjectBinding
	{
		public override string Name
		{
			get { return "Eto.FSharp"; }
		}

		static FSharpProjectBinding()
		{
		}

		protected override DotNetProject CreateProject(string languageName, ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{
			var project = base.CreateProject(languageName, info, projectOptions);
			// fix NRE when creating projects - fixed in monodevelop/master but not applied as of XS 5.9.4
			foreach (var config in project.Configurations.OfType<DotNetProjectConfiguration>())
			{
				var fsharpParameters = config.CompilationParameters as FSharpCompilerParameters;
				if (fsharpParameters != null)
				{
					if (fsharpParameters.DefineConstants == null)
						fsharpParameters.DefineConstants = string.Empty;
				}
			}
			return project;
		}
	}
}

