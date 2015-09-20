using System;
using MonoDevelop.Ide.Gui;
using Eto.Forms;
using MonoDevelop.Ide;
using MonoDevelop.Components.Commands;
using System.Reflection;
using System.IO;
using Eto.Designer;
using Mono.TextEditor;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Navigation;
using System.Linq;
using MonoDevelop.Core;
using System.Collections.Generic;

namespace Eto.Addin.XamarinStudio.Editor
{
	public class EditorView : AbstractViewContent
	{
		readonly IViewContent content;
		Gtk.Widget control;
		PreviewEditorView preview;

		public EditorView(IViewContent content)
		{
			this.content = content;
			preview = new PreviewEditorView(content.Control.ToEto(), () => content?.WorkbenchWindow?.Document?.Editor?.Text);
			content.ContentChanged += content_ContentChanged;
			content.DirtyChanged += content_DirtyChanged;
			var commandRouterContainer = new CommandRouterContainer(preview.ToNative(true), content, true);
			commandRouterContainer.Show();
			control = commandRouterContainer;
			IdeApp.Workbench.ActiveDocumentChanged += Workbench_ActiveDocumentChanged;

			base.ContentName = content.ContentName;
		}

		public override Gtk.Widget Control
		{
			get { return control; }
		}

		public override void Load(string fileName)
		{
			preview.SetBuilder(fileName);
			content.Load(fileName);
			base.ContentName = fileName;
		}

		public override bool CanReuseView(string fileName)
		{
			return content.CanReuseView(fileName);
		}

		public override string ContentName
		{
			get { return content.ContentName; }
			set
			{
				base.ContentName = value;
				content.ContentName = value; 
			}
		}

		public override void DiscardChanges()
		{
			content.DiscardChanges();
		}

		public override bool IsDirty
		{
			get { return content.IsDirty; }
			set
			{
				base.IsDirty = value;
				content.IsDirty = value;
			}
		}

		public override bool IsReadOnly
		{
			get { return content.IsReadOnly; }
		}

		public override void Save(string fileName)
		{
			content.Save(fileName);
		}

		public override MonoDevelop.Projects.Project Project
		{
			get { return base.Project; }
			set
			{
				base.Project = value;
				content.Project = value;
			}
		}

		protected override void OnWorkbenchWindowChanged(EventArgs e)
		{
			base.OnWorkbenchWindowChanged(e);
			if (content != null)
				content.WorkbenchWindow = WorkbenchWindow;
			if (WorkbenchWindow != null)
			{
				//WorkbenchWindow.AttachViewContent(this);
				WorkbenchWindow.ActiveViewContent = content;
				//WorkbenchWindow.ActiveViewContentChanged += WorkbenchWindow_ActiveViewContentChanged;
				WorkbenchWindow.DocumentChanged += WorkbenchWindow_DocumentChanged;
			}
		}

		IEnumerable<Gtk.Widget> GetChildren(Gtk.Widget widget)
		{
			var container = widget as Gtk.Container;
			if (container != null)
			{
				foreach (var child in container.AllChildren.OfType<Gtk.Widget>())
				{
					yield return child;
					foreach (var cc in GetChildren(child))
					{
						yield return cc;
					}
				}
			}
		}

		void Workbench_ActiveDocumentChanged(object sender, EventArgs e)
		{
			if (IdeApp.Workbench.ActiveDocument != null && IdeApp.Workbench.ActiveDocument.GetContent<EditorView>() == this)
			{
				/*var focus = GetChildren(content.Control).FirstOrDefault(r => r.CanFocus);
				if (focus != null)
					focus.GrabFocus();*/
			}
		}

		void WorkbenchWindow_DocumentChanged(object sender, EventArgs e)
		{
			var doc = WorkbenchWindow?.Document?.Editor?.Document;
			if (doc != null)
			{
				doc.LineChanged += (senderr, ee) => preview.Update();
				preview.Update();
			}
		}

		public override void Dispose()
		{
			content.ContentChanged -= content_ContentChanged;
			content.DirtyChanged -= content_DirtyChanged;
			IdeApp.Workbench.ActiveDocumentChanged -= Workbench_ActiveDocumentChanged;
			content.Dispose();
			base.Dispose();
		}

		void content_ContentChanged(object s, EventArgs args)
		{
			OnContentChanged(args);
		}

		void content_DirtyChanged(object s, EventArgs args)
		{
			OnDirtyChanged(args);
		}

		public override object GetContent(Type type)
		{
			return type.IsInstanceOfType(this) ? this : content?.GetContent(type);
		}
	}
}
