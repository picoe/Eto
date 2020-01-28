using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Documents;
using MonoDevelop.Projects;

namespace Eto.Addin.MonoDevelop.Editor
{
	[ExportDocumentControllerFactory(Id = "EtoFormsDesigner", MimeType = "*", InsertBefore = "MonoDevelop.TextEditor.CocoaTextViewDisplayBinding")]
	public class DesignerDisplayBinding : FileDocumentControllerFactory
	{
		bool enabled = true;

		static DesignerDisplayBinding()
		{
			EtoInitializer.Initialize();
		}

		protected override async Task<IEnumerable<DocumentControllerDescription>> GetSupportedControllersAsync(FileDescriptor file)
		{
			var list = ImmutableList<DocumentControllerDescription>.Empty;
			if (!enabled)
				return list;

			var info = Eto.Designer.BuilderInfo.Find(file.FilePath);
			if (info == null)
				return list;

			return list.Add(new DocumentControllerDescription
			{
				Name = "Eto.Forms Visual Designer",
				Role = DocumentControllerRole.VisualDesign,
				CanUseAsDefault = true
			});
		}

		public override async Task<DocumentController> CreateController(FileDescriptor file, DocumentControllerDescription controllerDescription)
		{
			enabled = false;
			var existing = await IdeServices.DocumentControllerService.CreateTextEditorController(file);
			enabled = true;
			return new EtoDesignerViewContent(existing);
		}

		public override string Id => "Eto.Addin.MonoDevelop.Editor.DesignerDisplayBinding";
	}

	class EtoDesignerViewContent : DocumentController
	{
		bool isClosed;
		FileDocumentController _inner;
		public EtoDesignerViewContent(FileDocumentController inner)
		{
			_inner = inner;
			_inner.ContentChanged += (sender, e) => OnContentChanged();
			_inner.DocumentIconChanged += (sender, e) => DocumentIcon = _inner.DocumentIcon;
			_inner.DocumentTitleChanged += (sener, e) => DocumentTitle = _inner.DocumentTitle;
			_inner.IsReadOnlyChanged += (sender, e) => IsReadOnly = _inner.IsReadOnly;
			_inner.IsNewDocumentChanged += (sender, e) => IsNewDocument = _inner.IsNewDocument;
			_inner.HasUnsavedChangesChanged += (sender, e) => HasUnsavedChanges = _inner.HasUnsavedChanges;
			_inner.TabPageLabelChanged += (sender, e) => TabPageLabel = _inner.TabPageLabel;
			//_inner.FilePathChanged += (sender, e) => FilePath = _inner.FilePath;
			_inner.OwnerChanged += (sender, e) => Owner = _inner.Owner;
			_inner.HasUnsavedChangesChanged += (sender, e) => HasUnsavedChanges = _inner.HasUnsavedChanges;
		}

		protected override bool ControllerIsViewOnly => _inner.IsViewOnly;

		protected override ProjectReloadCapability OnGetProjectReloadCapability() => _inner.ProjectReloadCapability;

		protected override bool OnCanAssignModel(Type type) => _inner.CanAssignModel(type);

		// last view mode
		static DocumentViewContainerMode s_lastMode = DocumentViewContainerMode.HorizontalSplit;
		static int s_lastView = 1;

		protected override async Task<DocumentView> OnInitializeView()
		{
			var container = new DocumentViewContainer();

			// can't set last mode here otherwise if we set Tabs, clicking Split incorrectly puts it into VerticalSplit mode
			container.SupportedModes = DocumentViewContainerMode.Tabs | DocumentViewContainerMode.HorizontalSplit;
			container.CurrentMode = DocumentViewContainerMode.HorizontalSplit;

			// our designer content
			var content = new DocumentViewContent(OnGetViewControlAsync);
			content.Title = "Design";
			container.Views.Add(content);

			// the source code editor
			var innerView = await _inner.GetDocumentView();
			container.Views.Add(innerView);

			return container;
		}

		protected override void OnOwnerChanged()
		{
			_inner.Owner = Owner;
			base.OnOwnerChanged();
		}

		protected override void OnModelChanged(DocumentModel oldModel, DocumentModel newModel)
		{
			_inner.Model = newModel;

			base.OnModelChanged(oldModel, newModel);
		}

		protected override Properties OnGetDocumentStatus() => _inner.GetDocumentStatus();

		protected override void OnSetDocumentStatus(Properties properties)
		{
			_inner.SetDocumentStatus(properties);
			base.OnSetDocumentStatus(properties);
		}

		protected override bool OnTryReuseDocument(ModelDescriptor modelDescriptor)
		{
			return _inner.TryReuseDocument(modelDescriptor);
		}

		protected override Task OnSave() => _inner.Save();

		protected override async Task OnInitialize(ModelDescriptor modelDescriptor, Properties status)
		{
			await _inner.Initialize(modelDescriptor, status);

			await base.OnInitialize(modelDescriptor, status);
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			SetLastMode();
		}

		protected override void OnGrabFocus(DocumentView view)
		{
			if (view is DocumentViewContainer container)
			{
				if (container.CurrentMode != DocumentViewContainerMode.Tabs)
				{
					// default focus to the code, not the designer
					container.Views[1].GrabFocus();
					return;
				}
			}
			base.OnGrabFocus(view);

		}

		protected override IEnumerable<FilePath> OnGetDocumentFiles() => _inner.GetDocumentFiles();

		protected override object OnGetContent(Type type) => _inner.GetContent(type);

		protected override IEnumerable<object> OnGetContents(Type type) => _inner.GetContents(type);

		Task<Control> OnGetViewControlAsync(CancellationToken token)
		{
			string assemblyFile = null;
			if (Owner is DotNetProject owner)
			{
				// we need appdomains for this!
				//assemblyFile = owner.GetOutputFileName(ConfigurationSelector.Default);
			}

			var preview = new Eto.Designer.PreviewEditorView(assemblyFile, Enumerable.Empty<string>(), () => new StreamReader(_inner.FileModel.GetContent()).ReadToEnd());
			_inner.FileModel.Changed += (sender, e) => preview.Update();
			preview.SetBuilder(_inner.FilePath);

			// gtk2 only for now.. 
			var nativeControl = Eto.Forms.Gtk2Helpers.ToNative(preview, true);
			nativeControl.ShowAll();
			var result = (Control)nativeControl;

			Eto.Forms.Application.Instance.AsyncInvoke(preview.Update);


			return Task.FromResult(result);

		}
		protected override void OnContentShown()
		{
			base.OnContentShown();

			var container = DocumentView as DocumentViewContainer;
			if (container != null)
			{
				container.CurrentMode = s_lastMode;
				if (s_lastMode == DocumentViewContainerMode.Tabs)
					container.ActiveView = container.Views[s_lastView];
			}
		}

		protected override void OnContentHidden()
		{
			base.OnContentHidden();

			SetLastMode();
		}

		private void SetLastMode()
		{
			var container = DocumentView as DocumentViewContainer;
			if (container != null)
			{
				s_lastMode = container.CurrentMode;
				s_lastView = ReferenceEquals(container.Views[0], container.ActiveView) ? 0 : 1;
			}
		}

		protected override void OnDispose()
		{
			_inner?.Dispose();
			_inner = null;
			base.OnDispose();
		}

	}

}
