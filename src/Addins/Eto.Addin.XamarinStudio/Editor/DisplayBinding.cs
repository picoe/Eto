using System;
using MonoDevelop.Ide.Gui;
using Eto.Forms;
using MonoDevelop.Ide;
using MonoDevelop.Components.Commands;
using System.Reflection;
using System.IO;
using Eto.Designer;
//using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace Eto.Addin.XamarinStudio.Editor
{
	public class DisplayBinding : IViewDisplayBinding, IDisplayBinding
	{
		bool exclude;

		ViewContent IViewDisplayBinding.CreateContent (FilePath fileName, string mimeType, Project ownerProject)
		{
			exclude = true;
			var defaultViewBinding = DisplayBindingService.GetDefaultViewBinding (fileName, mimeType, ownerProject);
			var content = defaultViewBinding.CreateContent (fileName, mimeType, ownerProject);
			var result = new EditorView (content);
			exclude = false;
			return result;
		}

		public string Name
		{
			get { return "Eto.Forms designer"; }
		}

		public bool CanHandle(MonoDevelop.Core.FilePath fileName, string mimeType, MonoDevelop.Projects.Project ownerProject)
		{
			if (exclude)
				return false;

			if (mimeType != null && mimeType == "application/x-xeto")
				return true;
			
			var info = Eto.Designer.BuilderInfo.Find(fileName);
			return info != null;
		}

		public bool CanUseAsDefault
		{
			get
			{
				return true;
			}
		}
	}
}

