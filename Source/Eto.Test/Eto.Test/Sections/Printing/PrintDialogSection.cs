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

		public PrintDialogSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (null, ShowPrintDialog (), null);
			layout.AddRow (null, PrintFromGraphics (), null);

			layout.Add (null);
		}

		Control ShowPrintDialog ()
		{
			var control = new Button { Text = "Show Print Dialog" };

			control.Click += delegate {
				var print = new PrintDialog ();
				var ret = print.ShowDialog (this.ParentWindow);
				if (ret == DialogResult.Ok) {
				}
			};

			return control;
		}
	
		Control PrintFromGraphics ()
		{
			var control = new Button { Text = "Print From Graphics" };

			control.Click += delegate {
				var document = new PrintDocument ();
				var font = new Font(FontFamilies.Serif, 16);
				var printTime = DateTime.Now;
				document.PrintPage += (sender, e) => {
					// draw title
					e.Graphics.DrawText (font, Colors.Black, new Point (50, 20), document.Name);

					// draw page number
					var text = string.Format ("page {0} of {1}", e.CurrentPage + 1, document.PageCount);
					var textSize = e.Graphics.MeasureString(font, text);
					e.Graphics.DrawText (font, Colors.Black, new Point (e.PageSize.Width - (int)textSize.Width - 50, 20), text);

					// draw date
					text = string.Format ("Printed on {0:f}", printTime);
					textSize = e.Graphics.MeasureString (font, text);
					e.Graphics.DrawText (font, Colors.Black, new Point (e.PageSize.Width - (int)textSize.Width - 50, e.PageSize.Height - (int)textSize.Height - 20), text);

					// draw some rectangles
					switch (e.CurrentPage) {
					case 0:
						e.Graphics.DrawRectangle (Colors.Blue, new Rectangle (50, 50, 100, 100));
						e.Graphics.DrawRectangle (Colors.Green, new Rectangle (new Point (e.PageSize) - new Size (150, 150), new Size (100, 100)));
						break;
					case 1:
						e.Graphics.DrawRectangle (Colors.Blue, new Rectangle (e.PageSize.Width - 150, 50, 100, 100));
						e.Graphics.DrawRectangle (Colors.Green, new Rectangle (50, e.PageSize.Height - 150, 100, 100));
						break;
					}
				};
				document.PrintSettings = new PrintSettings ();
				document.Name = "Name Of Document";
				document.PageCount = 2;
				document.Print ();
			};

			return control;
		}
	}
}
