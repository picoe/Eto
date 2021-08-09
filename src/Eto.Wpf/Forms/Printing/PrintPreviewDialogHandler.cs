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
			var tempFileName = Path.GetTempFileName();
			try
			{
				using (var previewDocument = documentHandler.GetPrintPreview(tempFileName))
				{
					Control.Document = previewDocument.GetFixedDocumentSequence();
					Control.ShowDialog();
				}
			}
			finally
			{
				if (File.Exists(tempFileName))
					File.Delete(tempFileName);
			}

			return DialogResult.Ok;
		}

		public PrintSettings PrintSettings
		{
			get => settings ?? (settings = new PrintSettings());
			set => settings = value;
		}
	}
}
