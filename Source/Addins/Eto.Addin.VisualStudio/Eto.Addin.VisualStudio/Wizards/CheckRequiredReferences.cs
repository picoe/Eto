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

namespace Eto.Addin.VisualStudio.Wizards
{
	public class CheckRequiredReferences : IWizard
	{
		public class ReferenceInfo
		{
			public string Assembly { get; set; }

			public string Version { get; set; }

			public string Package { get; set; }

			public ReferenceInfo(XmlElement element)
			{
				Package = element.GetAttribute("id");
				Assembly = element.GetAttribute("assembly");
				if (string.IsNullOrEmpty(Assembly))
					Assembly = Package;
				Version = element.GetAttribute("version");
				ExtensionId = element.GetAttribute("extension");
			}

			public string ExtensionId { get; set; }
		}

		public class MissingReference
		{
			public ReferenceInfo Reference { get; set; }

			public EnvDTE.Project Project { get; set; }
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var requiredReferences = GetRequiredReferences(replacementsDictionary);

	
			using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject))
			{
				var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
				var installerServices = componentModel.GetService<IVsPackageInstaller>();

				//installerServices.InstallPackage()
				//installerServices.InstallPackage("http://packages.nuget.org", project, "Microsoft.AspNet.SignalR.JS", "1.0.0-alpha2", false);

				var dte = (EnvDTE.DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));
				var missingReferences = FindMissingReferences(requiredReferences, installerServices, dte).ToList();

				if (missingReferences.Count > 0)
				{
					var packages = string.Join(", ", missingReferences.Select(r => r.Reference.Package));
					var msg = string.Format("This template requires these references. Do you want to add them using nuget?\n\n{0}", packages);

					var result = MessageBox.Show(Helpers.MainWindow, msg, "Missing packages", MessageBoxButtons.OKCancel, MessageBoxType.Information, MessageBoxDefaultButton.OK);
					if (result == DialogResult.Cancel)
						throw new WizardCancelledException();

					foreach (var missingRef in missingReferences)
					{
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
							installerServices.InstallPackage("https://packages.nuget.org", missingRef.Project, reference.Package, reference.Version, false);
						}
					}
				}

			}

		}

		static IEnumerable<MissingReference> FindMissingReferences(IEnumerable<ReferenceInfo> requiredReferences, IVsPackageInstaller installerServices, EnvDTE.DTE dte)
		{
			var activeProjects = ((Array)dte.ActiveSolutionProjects).OfType<EnvDTE.Project>().ToList();
			foreach (var proj in activeProjects)
			{
				var vsproject = proj.Object as VSLangProj.VSProject;
				var references = vsproject.References.OfType<VSLangProj.Reference>().ToList();
				foreach (var requiredRef in requiredReferences)
				{
					if (!references.Any(r => r.Name == requiredRef.Assembly))
					{
						yield return new MissingReference { Project = proj, Reference = requiredRef };
					}
				}
			}
		}

		static IEnumerable<ReferenceInfo> GetRequiredReferences(Dictionary<string, string> replacementsDictionary)
		{
			var wizardData = "<root>" + replacementsDictionary["$wizarddata$"] + "</root>";
			var doc = new XmlDocument();
			doc.LoadXml(wizardData);
			var nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("vs", "http://schemas.microsoft.com/developer/vstemplate/2005");
			var requiredReferences = doc.SelectNodes("//vs:RequiredReferences/vs:Reference", nsmgr).OfType<XmlElement>().Select(r => new ReferenceInfo(r));
			return requiredReferences;
		}

		public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
		{
		}

		public void ProjectFinishedGenerating(EnvDTE.Project project)
		{
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

