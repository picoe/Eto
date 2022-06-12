using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class PrintingTests : TestBase
	{
		[TestCase(10)]
		[TestCase(1000)]
		[InvokeOnUI]
		public void PrintControl(int count)
		{
			var ctl = new TableLayout
			{
				Rows = {
					new TableRow(new Label { Text = "Hello"}),
					new TextBox { Text = "Some Text"},
				}
			};

			for (int i = 0; i < count; i++)
			{
				ctl.Rows.Add(new Label { Text = "Row " + i });
			}

			ctl.Rows.Add(new TableRow { ScaleHeight = true });
			ctl.Rows.Add(new Label { Text = "Bottom" });

			ctl.Print();
		}

		[TestCase(10)]
		[TestCase(1000)]
		[InvokeOnUI]
		public void PrintControlPreview(int count)
		{
			var ctl = new TableLayout
			{
				Rows = {
					new TableRow(new Label { Text = "Hello"}),
					new TextBox { Text = "Some Text"}
				}
			};

			for (int i = 0; i < count; i++)
			{
				ctl.Rows.Add(new Label { Text = "Row " + i });
			}

			ctl.Rows.Add(new TableRow { ScaleHeight = true });
			ctl.Rows.Add(new Label { Text = "Bottom" });
			
			var doc = new PrintDocument(ctl);
			doc.PrintPage += (sender, e) =>
			{
				// add page number to top right of page
				var text = $"Page {e.CurrentPage}";
				var textSize = e.Graphics.MeasureString(SystemFonts.Default(), text);
				e.Graphics.DrawText(SystemFonts.Default(), Colors.Black, e.PageSize.Width - textSize.Width, 0, text);
			};

			var dlg = new PrintPreviewDialog(doc);
			dlg.ShowDialog(null);

			doc.Dispose();
		}
		
		[Test]
		[InvokeOnUI]
		public void PrintPreviewWithGraphics()
		{
			var settings = new PrintSettings();
			var doc = Sections.Printing.PrintDialogSection.GetPrintDocument(settings);
			var dlg = new PrintPreviewDialog(doc);
			dlg.ShowDialog(null);
			doc.Dispose();
		}
		
		[Test]
		[InvokeOnUI]
		public void PrintWithGraphics()
		{
			var settings = new PrintSettings();
			var doc = Sections.Printing.PrintDialogSection.GetPrintDocument(settings);
			var dlg = new PrintDialog();
			dlg.ShowDialog(null, doc);
			doc.Dispose();
		}
		
		[Test]
		[InvokeOnUI]
		public void PrintDialogWithoutDocument()
		{
			var settings = new PrintSettings();
			var dlg = new PrintDialog();
			dlg.ShowDialog(null);
		}
	}
}