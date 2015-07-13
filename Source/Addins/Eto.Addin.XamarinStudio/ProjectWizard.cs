using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using MonoDevelop.Projects;
using System.Text;
using System.Linq;
using MonoDevelop.FSharp;

namespace Eto.Addin.XamarinStudio
{
	public class ProjectWizard : TemplateWizard
	{
		static ProjectWizard()
		{
			EtoInitializer.Initialize();

			MonoDevelop.Ide.IdeApp.ProjectOperations.ProjectCreated += (sender, e) =>
			{
					Console.WriteLine("woo");
				// Fix FSharp NRE until XS is fixed
				/*var proj = e.SolutionItem as DotNetProject;
				if (proj != null)
				{
					foreach (var config in proj.Configurations.OfType<DotNetProjectConfiguration>())
					{
						var fsharpParameters = config.CompilationParameters as FSharpCompilerParameters;
						if (fsharpParameters != null)
						{
							if (fsharpParameters.DefineConstants == null)
								fsharpParameters.DefineConstants = string.Empty;
						}
					}
				}*/
			};
		}

		public override void ConfigureWizard()
		{
			Parameters["BaseProjectName"] = Parameters["ProjectName"].Trim();
			base.ConfigureWizard();
		}

		public override string Id
		{
			get { return "Eto.Addin.XamarinStudio.ProjectWizard"; }
		}

		public bool ShowGenerateCombined
		{
			get { return IsSupportedParameter("GenerateCombined"); }
		}

		public bool ShowSharedCode
		{
			get { return IsSupportedParameter("SharedCode"); }
		}

		public bool SupportsPCL
		{
			get { return IsSupportedParameter("SupportsPCL"); }
		}

		public override WizardPage GetPage(int pageNumber)
		{
			return new ProjectWizardPage(this);
		}
	}
}

