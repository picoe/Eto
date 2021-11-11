using System;
using System.IO;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms.Printing
{
	public class EtoPrintPreviewDialog : sw.Window
	{
		swc.DocumentViewer _documentViewer;
		
		public sw.Documents.IDocumentPaginatorSource Document
		{
			get => _documentViewer.Document;
			set => _documentViewer.Document = value;
		}
		
		public EtoPrintPreviewDialog()
		{
			_documentViewer = new swc.DocumentViewer();
			Content = _documentViewer;
			ShowInTaskbar = false;
		}
	}
	
	public class PrintPreviewDialogHandler : WidgetHandler<EtoPrintPreviewDialog, PrintPreviewDialog>, PrintPreviewDialog.IHandler
	{
		PrintSettings settings;
		public PrintPreviewDialogHandler()
		{
			Control = new EtoPrintPreviewDialog();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			var print = new swc.PrintDialog();
			print.SetEtoSettings(PrintSettings);
			Control.Owner = parent?.ToNative();
			
			if (!string.IsNullOrEmpty(Document.Name))
				Control.Title = string.Format(Application.Instance.Localize(Widget, "Print Preview - {0}"), Document.Name);
			else
				Control.Title = Application.Instance.Localize(Widget, "Print Preview");

			var printCapabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
			var ia = printCapabilities.PageImageableArea;
			var documentHandler = (PrintDocumentHandler)Document.Handler;
			var tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); // Path.GetTempFileName creates a file, we don't want it created yet.
			try
			{
				using (var previewDocument = documentHandler.GetPrintPreview(tempFileName))
				{
					var fixedDocument = previewDocument.GetFixedDocumentSequence();
					fixedDocument.PrintTicket = print.PrintTicket;
					// fixedDocument.DocumentPaginator.PageSize = new sw.Size(1000, 800);
					
					Control.Document = fixedDocument;
					Control.ShowDialog();
				}
			}
			finally
			{
				if (File.Exists(tempFileName))
				{
					try
					{
						File.Delete(tempFileName);
					}
					catch
					{
						// ignoring errors
					}
				}
			}

			return DialogResult.Ok;
		}

		public PrintSettings PrintSettings
		{
			get => settings ?? (settings = Document.PrintSettings ?? new PrintSettings());
			set => settings = value;
		}
	}
}
