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
using System.Reflection;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class KeepFSharpOrderWizard : BaseWizard
	{
		List<ProjectItem> items;
        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
		{
			base.ProjectItemFinishedGenerating(projectItem);
			if (items == null)
				items = new List<ProjectItem>();
			items.Add(projectItem);
        }

		public override void RunFinished()
		{
			base.RunFinished();
			if (items == null)
				return;
			// hack to keep proper order of F# files when using multi-file item templates.
			foreach (var projectItem in items)
			{
				var type = projectItem.GetType();
				Func<object, string, object[], object> invoke = (obj, member, args) => obj.GetType().InvokeMember(member, BindingFlags.InvokeMethod, null, obj, args);

				var node = invoke(projectItem, "get_Node", null);
				if (node == null)
					return;
				var root = invoke(node, "get_Parent", null);
				if (root == null)
					return;
				var moveUpMethod = node.GetType().GetMethod("MoveUp", BindingFlags.Static | BindingFlags.NonPublic);
				if (moveUpMethod == null)
					return;
				var moveDownMethod = node.GetType().GetMethod("MoveDown", BindingFlags.Static | BindingFlags.NonPublic);
				if (moveDownMethod == null)
					return;

				moveUpMethod.Invoke(null, new object[] { node, root });
				moveDownMethod.Invoke(null, new object[] { node, root });
			}
			items = null;
		}
	}
}
