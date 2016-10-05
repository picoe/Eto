using Eto.Forms;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel.Composition;
using NuGet.VisualStudio;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class CheckRequiredReferences : IWizard
	{
		public class ReferenceInfo
		{
			public string Assembly { get; set; }

			public string Version { get; set; }

			public string Package { get; set; }

			public string Condition { get; set; }

			public ReferenceInfo(XmlElement element)
			{
				Package = element.GetAttribute("id");
				Assembly = element.GetAttribute("assembly");
				if (string.IsNullOrEmpty(Assembly))
					Assembly = Package;
				Version = element.GetAttribute("version");
				ExtensionId = element.GetAttribute("extension");
				Condition = element.GetAttribute("condition");
			}

			public string ExtensionId { get; set; }
		}

		public bool Quiet { get; set; }

		public class MissingReference
		{
			public ReferenceInfo Reference { get; set; }

			public EnvDTE.Project Project { get; set; }
		}

		List<ReferenceInfo> requiredReferences;
		object automationObject;

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			requiredReferences = GetRequiredReferences(replacementsDictionary).ToList();
			if (runKind == WizardRunKind.AsNewItem)
			{
				using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject))
				{
					var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
					var installerServices = componentModel.GetService<IVsPackageInstaller>();
					var dte = (EnvDTE.DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));

					var missingReferences = FindMissingReferences(installerServices, dte).ToList();
					InstallReferences(installerServices, missingReferences);
				}
			}
			else
				this.automationObject = automationObject;

		}

		private void InstallReferences(IVsPackageInstaller installerServices, List<MissingReference> missingReferences)
		{
			if (missingReferences.Count > 0)
			{
				var packages = string.Join(", ", missingReferences.Select(r => r.Reference.Package));
				var msg = string.Format("This template requires these references. Do you want to add them using nuget?\n\n{0}", packages);

				if (!Quiet)
				{
					var result = MessageBox.Show(Helpers.MainWindow, msg, "Missing packages", MessageBoxButtons.YesNoCancel, MessageBoxType.Information, MessageBoxDefaultButton.Yes);
					if (result == DialogResult.Cancel)
						throw new WizardCancelledException();
					if (result == DialogResult.No)
						return;
				}

				foreach (var missingRef in missingReferences)
				{
					SetStatusMessage(string.Format("Adding {0}.{1} to project...", missingRef.Reference.Package, missingRef.Reference.Version));
					var reference = missingRef.Reference;
					if (!string.IsNullOrEmpty(reference.ExtensionId))
					{
						installerServices.InstallPackagesFromVSExtensionRepository(reference.ExtensionId, false, false, false, missingRef.Project, new Dictionary<string, string>
							{
								{ reference.Package, reference.Version }
							});
					}
					else
					{
						// "All" specifies to use configured package sources.
						// http://blog.nuget.org/20120926/invoking-nuget-services-from-inside-visual-studio.html#comment-686825894
						installerServices.InstallPackage("All", missingRef.Project, reference.Package, reference.Version, false);
					}
				}
			}
		}

		IEnumerable<MissingReference> FindMissingReferences(IVsPackageInstaller installerServices, EnvDTE.DTE dte)
		{
			var activeProjects = ((Array)dte.ActiveSolutionProjects).OfType<EnvDTE.Project>().ToList();
			foreach (var proj in activeProjects)
			{
				foreach (var item in FindMissingReferences(proj))
					yield return item;
			}
		}

		IEnumerable<MissingReference> FindMissingReferences(Project proj)
		{
			var vsproject = proj.Object as VSLangProj.VSProject;
			var references = vsproject.References.OfType<VSLangProj.Reference>().ToList();
			foreach (var requiredRef in requiredReferences)
			{
				if (string.IsNullOrEmpty(requiredRef.Assembly) || !references.Any(r => r.Name == requiredRef.Assembly))
				{
					yield return new MissingReference { Project = proj, Reference = requiredRef };
				}
			}
		}

		IEnumerable<ReferenceInfo> GetRequiredReferences(Dictionary<string, string> replacementsDictionary)
		{
			var wizardData = "<root>" + replacementsDictionary["$wizarddata$"] + "</root>";
			var doc = new XmlDocument();
			doc.LoadXml(wizardData);
			var nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("vs", "http://schemas.microsoft.com/developer/vstemplate/2005");
			var references = doc.SelectSingleNode("//vs:RequiredReferences", nsmgr) as XmlElement;
			Quiet = string.Equals(references.GetAttribute("Quiet"), "true", StringComparison.OrdinalIgnoreCase);
			var requiredReferences = references.SelectNodes("vs:Reference", nsmgr)
				.OfType<XmlElement>()
				.Select(r => new ReferenceInfo(r))
				.Where(r => replacementsDictionary.MatchesCondition(r.Condition));
			return requiredReferences;
		}

		public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
		{
		}

		static IVsStatusbar StatusBar;

		internal static void SetStatusMessage(string message)
		{
			if (StatusBar == null)
			{
				StatusBar = Package.GetGlobalService(typeof(IVsStatusbar)) as IVsStatusbar;
			}

			StatusBar.SetText(message);
		}

		public void ProjectFinishedGenerating(EnvDTE.Project project)
		{
			if (automationObject != null)
			{
				using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject))
				{
					var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
					var installerServices = componentModel.GetService<IVsPackageInstaller>();

					var missingReferences = FindMissingReferences(installerServices, project.DTE).ToList();
					InstallReferences(installerServices, missingReferences);
				}
				automationObject = null;
			}
		}

		public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
		{
		}

		public void RunFinished()
		{
		}

		public bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}
	}
}

