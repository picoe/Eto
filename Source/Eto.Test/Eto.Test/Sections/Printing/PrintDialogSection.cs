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
				document.PageCount = 10;
				document.PrintPage += (sender, e) => {
					if (e.CurrentPage == 0) {
						e.Graphics.DrawRectangle (Colors.Blue, new Rectangle(50, 50, 100, 100));
						e.Graphics.DrawRectangle (Colors.Green, new Rectangle(new Point(e.PageSize) - new Size(150, 150), new Size(100, 100)));
					}
				};
				document.PrintSettings = new PrintSettings ();
				//document.PageSize = new Size (600, 800);
				document.Name = "Name Of Document";
				document.PageCount = 2;
				document.Print ();
			};

			return control;
		}
	}
}
