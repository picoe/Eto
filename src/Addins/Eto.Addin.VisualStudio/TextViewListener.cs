using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using Eto.Designer;
using Microsoft.VisualStudio.Language.Intellisense;
using Eto.Addin.VisualStudio.Intellisense;
using Eto.Addin.VisualStudio.Util;

namespace Eto.Addin.VisualStudio
{

	[Export(typeof(IWpfTextViewCreationListener))]
	[Export(typeof(TextViewListener))]		// To let unit tests modify the instance
	[ContentType("text")]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	public class TextViewListener : IWpfTextViewCreationListener
	{
		[Import]
		public SVsServiceProvider ServiceProvider { get; set; }
		[Import]
		public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }
		[Import]
		public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }
		[Import]
		internal ICompletionBroker CompletionBroker { get; set; }

		public TextViewListener()
		{
			Instance = this;
		}

		public static TextViewListener Instance { get; private set; }

		static readonly object ViewFormKey = new object();
		static readonly object ViewCodeKey = new object();

		public void TextViewCreated(IWpfTextView textView)
		{
			ITextDocument document;
			if (!TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
				return;

			var textViewAdapter = EditorAdaptersFactoryService.GetViewAdapter(textView);
			if (textViewAdapter == null)
				return;

			var info = BuilderInfo.Find(document.FilePath);
			if (info != null)
			{
				// add commands to view form or code
				//textView.Properties.AddProperty(ViewFormKey, new AdapterCommand(textViewAdapter, ServiceProvider, VSConstants.GUID_VSStandardCommandSet97, (uint)VSConstants.VSStd97CmdID.ViewForm, () => ViewDesigner(document)));
				//textView.Properties.AddProperty(ViewCodeKey, new AdapterCommand(textViewAdapter, ServiceProvider, VSConstants.GUID_VSStandardCommandSet97, (uint)VSConstants.VSStd97CmdID.ViewCode, () => ViewCode(document)));
				if (string.Equals(info.Extension, ".xeto", StringComparison.OrdinalIgnoreCase))
					textView.Properties.GetOrCreateSingletonProperty(() => new XamlCompletionHandler(textViewAdapter, textView, this));
			}
			else if (BuilderInfo.IsCodeBehind(document.FilePath))
			{
				textView.Properties.AddProperty(ViewFormKey, new AdapterCommand(textViewAdapter, ServiceProvider, VSConstants.GUID_VSStandardCommandSet97, (uint)VSConstants.VSStd97CmdID.ViewForm, () => ViewDesigner(document)));
			}
		}

		void ViewCode(ITextDocument document)
		{
			var builderInfo = BuilderInfo.Find(document.FilePath);
			if (builderInfo == null)
				return;
			var codeFile = builderInfo.GetCodeFile(document.FilePath);

			if (!File.Exists(codeFile))
				return;

			IVsWindowFrame frame;
			IVsUIHierarchy hierarchy;
			uint itemid;
			if (!VsShellUtilities.IsDocumentOpen(ServiceProvider, codeFile, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame)
				&& !VsShellUtilities.IsDocumentOpen(ServiceProvider, codeFile, VSConstants.LOGVIEWID.TextView_guid, out hierarchy, out itemid, out frame))
			{
				VsShellUtilities.OpenDocumentWithSpecificEditor(ServiceProvider, codeFile, VSConstants.VsEditorFactoryGuid.TextEditor_guid, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame);
			}
			ErrorHandler.ThrowOnFailure(frame.Show());
		}

		void ViewDesigner(ITextDocument document)
		{
			var builderInfo = BuilderInfo.FindCodeBehind(document.FilePath);
			if (builderInfo == null)
				return;
			var codeFile = builderInfo.GetDesignFile(document.FilePath);

			if (!File.Exists(codeFile))
				return;

			IVsWindowFrame frame;
			IVsUIHierarchy hierarchy;
			uint itemid;
			VsShellUtilities.OpenDocumentWithSpecificEditor(ServiceProvider, codeFile, Constants.EtoPreviewEditorFactory_guid, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame);
			ErrorHandler.ThrowOnFailure(frame.Show());
		}

		void ViewDesignSource(ITextDocument document)
		{
			var builderInfo = BuilderInfo.FindCodeBehind(document.FilePath);
			if (builderInfo == null)
				return;
			var codeFile = builderInfo.GetDesignFile(document.FilePath);

			if (!File.Exists(codeFile))
				return;

			IVsWindowFrame frame;
			IVsUIHierarchy hierarchy;
			uint itemid;
			if (!VsShellUtilities.IsDocumentOpen(ServiceProvider, codeFile, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame)
				&& !VsShellUtilities.IsDocumentOpen(ServiceProvider, codeFile, VSConstants.LOGVIEWID.TextView_guid, out hierarchy, out itemid, out frame))
			{
				VsShellUtilities.OpenDocumentWithSpecificEditor(ServiceProvider, codeFile, VSConstants.VsEditorFactoryGuid.TextEditor_guid, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame);
			}
			ErrorHandler.ThrowOnFailure(frame.Show());
		}
	}
}
