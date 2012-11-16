using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Printing
{
	public class PrintDialogSection : Panel
	{
		PrintSettings settings = new PrintSettings();

		public PrintDialogSection ()
		{
			this.DataContext = settings;

			var layout = new DynamicLayout (this);

			layout.AddSeparateRow (null, ShowPrintDialog (), null);
			layout.AddSeparateRow (null, PrintFromGraphicsWithDialog (), null);
			layout.AddSeparateRow (null, PrintFromGraphics (), null);
			layout.AddSeparateRow (null, Settings (), null);

			layout.Add (null);
		}

		Control ShowPrintDialog ()
		{
			var control = new Button { Text = "Show Print Dialog" };

			control.Click += delegate {
				var print = new PrintDialog { PrintSettings = settings };
				var ret = print.ShowDialog (this.ParentWindow);
				if (ret == DialogResult.Ok) {
					this.DataContext = settings = print.PrintSettings;
				}
			};

			return control;
		}

		PrintDocument GetPrintDocument ()
		{
			var document = new PrintDocument ();
			document.PrintSettings = settings;
			var font = new Font (FontFamilies.Serif, 16);
			var printTime = DateTime.Now;
			document.PrintPage += (sender, e) => {
				Size pageSize = Size.Round (e.PageSize);

				// draw a border around the printable area
				var rect = new Rectangle (pageSize);
				rect.Inflate (-1, -1);
				e.Graphics.DrawRectangle (Colors.Silver, rect);

				// draw title
				e.Graphics.DrawText (font, Colors.Black, new Point (50, 20), document.Name);

				// draw page number
				var text = string.Format ("page {0} of {1}", e.CurrentPage + 1, document.PageCount);
				var textSize = Size.Round (e.Graphics.MeasureString (font, text));
				e.Graphics.DrawText (font, Colors.Black, new Point (pageSize.Width - textSize.Width - 50, 20), text);

				// draw date
				text = string.Format ("Printed on {0:f}", printTime);
				textSize = Size.Round (e.Graphics.MeasureString (font, text));
				e.Graphics.DrawText (font, Colors.Black, new Point (pageSize.Width - textSize.Width - 50, pageSize.Height - textSize.Height - 20), text);

				// draw some rectangles
				switch (e.CurrentPage) {
				case 0:
					e.Graphics.DrawRectangle (Colors.Blue, new Rectangle (50, 50, 100, 100));
					e.Graphics.DrawRectangle (Colors.Green, new Rectangle (new Point (pageSize) - new Size (150, 150), new Size (100, 100)));
					break;
				case 1:
					e.Graphics.DrawRectangle (Colors.Blue, new Rectangle (pageSize.Width - 150, 50, 100, 100));
					e.Graphics.DrawRectangle (Colors.Green, new Rectangle (50, pageSize.Height - 150, 100, 100));
					break;
				}
			};
			document.Name = "Name Of Document";
			document.PageCount = 2;
			return document;
		}
	
		Control PrintFromGraphics ()
		{
			var control = new Button { Text = "Print From Graphics" };

			control.Click += delegate {
				var document = GetPrintDocument ();
				document.Print ();
			};

			return control;
		}

		Control PrintFromGraphicsWithDialog ()
		{
			var control = new Button { Text = "Print From Graphics With Dialog" };

			control.Click += delegate {
				var document = GetPrintDocument ();
				document.ShowPrintDialog (this);
				this.DataContext = settings = document.PrintSettings;
			};

			return control;
		}

		Control Settings ()
		{
			var layout = new DynamicLayout (new GroupBox { Text = "Settings" });

			layout.AddRow (new Label { Text = "Orientation" }, PageOrientation ());
			layout.AddRow (new Label { Text = "Copies" }, Copies ());
			layout.AddRow (null, Collate ());
			layout.AddRow (new Label { Text = "Min Page" }, StartPage (), new Label { Text = "Page Count" }, PageCount ());
			layout.AddRow (new Label { Text = "Start Page" }, StartPage (), new Label { Text = "End Page" }, PageCount ());

			return layout.Container;
		}

		Control PageOrientation ()
		{
			var control = new EnumComboBox<PageOrientation> ();
			control.Bind ("SelectedValue", "Orientation");
			return control;
		}

		Control Copies ()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.Bind ("Value", "Copies", defaultWidgetValue: 1);
			return control;
		}

		Control Collate ()
		{
			var control = new CheckBox { Text = "Collate" };
			control.Bind ("Checked", "Collate");
			return control;
		}

		Control StartPage ()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate {
				control.Value = settings.PageRange.Location;
			};
			control.ValueChanged += delegate {
				var range = settings.PageRange;
				range.Location = (int)control.Value;
				settings.PageRange = range;
			};
			return control;
		}

		Control PageCount ()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate {
				control.Value = settings.PageRange.Length;
			};
			control.ValueChanged += delegate {
				var range = settings.PageRange;
				range.Length = (int)control.Value;
				settings.PageRange = range;
			};
			return control;
		}

	}
}
