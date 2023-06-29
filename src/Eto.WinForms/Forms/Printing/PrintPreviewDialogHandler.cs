using sdp = System.Drawing.Printing;
namespace Eto.WinForms.Forms.Printing
{
	public class PrintPreviewDialogHandler : WidgetHandler<swf.PrintPreviewDialog, PrintPreviewDialog>, PrintPreviewDialog.IHandler
	{
		PrintSettings _printSettings;

		public PrintPreviewDialogHandler()
		{
			Control = new swf.PrintPreviewDialog();
			Control.ClientSize = new sd.Size(800, 600);

			ReplacePrintButton();
		}

		private void ReplacePrintButton()
		{
			var toolStrip = Control.Controls.OfType<swf.ToolStrip>().FirstOrDefault();
			if (toolStrip == null)
				return;

			var oldPrintButton = toolStrip.Items.OfType<swf.ToolStripButton>().FirstOrDefault(r => r.Name == "printToolStripButton");
			if (oldPrintButton == null)
				return;
				
			var printButton = new swf.ToolStripButton();
			printButton.Image = toolStrip.ImageList.Images[oldPrintButton.ImageIndex];
			printButton.DisplayStyle = swf.ToolStripItemDisplayStyle.Image;
			printButton.Click += printButton_Click;
			toolStrip.Items.RemoveAt(0);
			toolStrip.Items.Insert(0, printButton);
		}

		private void printButton_Click(object sender, EventArgs e)
		{
			var printDialog = new swf.PrintDialog();
			printDialog.Document = Control.Document;

			if (printDialog.ShowDialog() == swf.DialogResult.OK)
			{
				Control.Document.PrinterSettings = printDialog.PrinterSettings;
				Control.Document.Print();
			}
		}

		public PrintDocument Document { get; set; }
		public PrintSettings PrintSettings
		{
			get => _printSettings ?? (_printSettings = Document?.PrintSettings ?? new PrintSettings());
			set => _printSettings = value;
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

			swf.DialogResult result;
			Control.Document = PrintDocumentHandler.GetControl(Document);

			if (parent != null)
				result = Control.ShowDialog(((IWindowHandler)parent.Handler).Win32Window);
			else
				result = Control.ShowDialog();

			if (result == swf.DialogResult.OK && Document != null)
			{
				if (_printSettings != null)
					Document.PrintSettings = _printSettings;
				Document.Print();
			}

			return result.ToEto();
		}

	}
}
