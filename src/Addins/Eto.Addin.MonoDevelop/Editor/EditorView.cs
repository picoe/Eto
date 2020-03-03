#if MD_7_0
using System;
using MonoDevelop.Ide.Gui;
using Eto.Forms;
using MonoDevelop.Ide;
using MonoDevelop.Components.Commands;
using System.Reflection;
using System.IO;
using Eto.Designer;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Navigation;
using System.Linq;
using MonoDevelop.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
//using MonoDevelop.Components.Mac;
using System.Diagnostics;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Core.Text;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using md = MonoDevelop;

namespace Eto.Addin.MonoDevelop.Editor
{

	public class EditorView : ViewContent, ICommandRouter
	{
		static EditorView()
		{
			EtoInitializer.Initialize();
		}

		ViewContent content;
		md.Components.Control control;
		PreviewEditorView _preview;
		protected PreviewEditorView Preview => _preview ?? (_preview = GetPreview());

		protected Gtk.Widget EditorWidget => content.Control.GetNativeWidget<Gtk.Widget>();

		public EditorView(ViewContent content)
		{
			this.content = content;

			content.DirtyChanged += content_DirtyChanged;
			//IdeApp.Workbench.ActiveDocumentChanged += Workbench_ActiveDocumentChanged;
			ContentName = content.ContentName;
			Project = content.Project;
		}

		protected string GetEditorText()
		{
			return content?.WorkbenchWindow?.Document?.Editor?.Text;
		}

		protected virtual PreviewEditorView GetPreview()
		{
			var editorWidget = EditorWidget;
			editorWidget.ShowAll();
			return new PreviewEditorView(editorWidget.ToEto(), null, null, GetEditorText);
		}

		protected virtual global::MonoDevelop.Components.Control GetNativeControl()
		{
			return Gtk2Helpers.ToNative(Preview, true);
		}

		protected virtual md.Components.Control GetControl()
		{
			if (control != null)
				return control;
			var commandRouterContainer = new CommandRouterContainer(GetNativeControl(), content, true);
			commandRouterContainer.ShowAll();
			control = commandRouterContainer;
			return control;
		}

		public override async Task Load(FileOpenInformation fileOpenInformation)
		{
			await content.Load(fileOpenInformation);
			ContentName = fileOpenInformation.FileName;
			Project = fileOpenInformation.Project;
			IsDirty = false;
		}

		public override ProjectReloadCapability ProjectReloadCapability => content.ProjectReloadCapability;

		protected override void OnContentNameChanged()
		{
			base.OnContentNameChanged();
			content.ContentName = ContentName;
			Preview.SetBuilder(ContentName);
			Preview.Update();
		}

		protected override IEnumerable<object> OnGetContents(Type type) => content.GetContents(type);

		public override Task Save() => content.Save();

		public override string TabPageLabel => content.TabPageLabel;

		public override global::MonoDevelop.Components.Control Control => GetControl();

		public override bool CanReuseView(string fileName) => content.CanReuseView(fileName);

		public override void DiscardChanges() => content.DiscardChanges();

		public override bool IsDirty
		{
			get { return content.IsDirty; }
			set
			{
				base.IsDirty = value;
				content.IsDirty = value;
			}
		}

		public override bool IsReadOnly => content.IsReadOnly;

		public override async Task Save(FileSaveInformation fileSaveInformation)
		{
			var mimeType = WorkbenchWindow?.Document?.Editor?.MimeType;
			await content.Save(fileSaveInformation.FileName);
			ContentName = fileSaveInformation.FileName;

			// work around bug in VS when you save it reverts to plain text
			// so, we restore the mime type to its original type.
			if (mimeType != null)
			{
				var editor = WorkbenchWindow?.Document.Editor;
				if (editor != null)
					editor.MimeType = mimeType;
			}
			IsDirty = false;
		}

		protected override void OnSetProject(md.Projects.Project project)
		{
			base.OnSetProject(project);
			if (content.Project != project)
				content.Project = project;
		}

		protected override void OnDirtyChanged()
		{
			base.OnDirtyChanged();
			content.IsDirty = IsDirty;
		}

		protected override void OnWorkbenchWindowChanged()
		{
			base.OnWorkbenchWindowChanged();
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
			var doc = WorkbenchWindow?.Document;
			if (doc != null)
			{
				doc.Editor.TextChanged += Editor_TextChanged;
				Preview.Update();
			}
		}

		void Editor_TextChanged(object sender, md.Core.Text.TextChangeEventArgs e)
		{
			Preview.Update();
		}

		public override void Dispose()
		{
			if (content != null)
			{
				content.DirtyChanged -= content_DirtyChanged;
				IdeApp.Workbench.ActiveDocumentChanged -= Workbench_ActiveDocumentChanged;
				content.Dispose();
				content = null;
			}
			base.Dispose();
		}

		void content_DirtyChanged(object s, EventArgs args) => OnDirtyChanged();

		protected override object OnGetContent(Type type)
		{
			return type.IsInstanceOfType(this) ? this : content?.GetContent(type);
		}

		public override bool IsFile => content.IsFile;

		public override object GetDocumentObject() => content.GetDocumentObject();

		object ICommandRouter.GetNextCommandTarget() => (content as ICommandRouter)?.GetNextCommandTarget();
	}
}

#endif