using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	public class SpacingSection : Panel
	{
		Size SquareSize = new Size (30, 40);
		public SpacingSection ()
		{
			var layout = new TableLayout (this, 3, 1);

			layout.Add (NoSpacing (), 0, 0, true, true);
			layout.Add (NoPadding (), 1, 0, true, true);
			layout.Add (DifferentSizes (), 2, 0, true, true);
		}

		void FillTable (TableLayout layout)
		{
			for (int y = 0; y < layout.Size.Height; y++)
				for (int x = 0; x < layout.Size.Width; x++) {
					var panel = new Panel { 
						Size = SquareSize, 
						BackgroundColor = (x+y*layout.Size.Width) % 2 == 0 ? Colors.Lime : Colors.Red 
					};
					layout.Add (panel, x, y);
				}
		}

		Control NoSpacing ()
		{
			var layout = new TableLayout (new Panel { BackgroundColor = Colors.Blue }, 3, 3);
			layout.Padding = new Padding (10);
			layout.Spacing = Size.Empty;
			layout.SetColumnScale (1); // scale middle column
			layout.SetRowScale (1); // scale middle row
			FillTable (layout);
			return layout.Container;
		}

		Control DifferentSizes ()
		{
			var layout = new TableLayout (new Panel { BackgroundColor = Colors.Blue }, 3, 4);
			// row 1
			layout.Add (new Panel { Size = new Size (10, 10), BackgroundColor = Colors.Lime }, 0, 0);
			layout.Add (new Panel { Size = new Size (30, 10), BackgroundColor = Colors.Red }, 1, 0);
			layout.Add (new Panel { Size = new Size (10, 30), BackgroundColor = Colors.Lime }, 2, 0);

			// row 2
			layout.Add (new Panel { Size = new Size (30, 10), BackgroundColor = Colors.Red }, 0, 1);
			layout.Add (new Panel { Size = new Size (10, 30), BackgroundColor = Colors.Lime }, 1, 1);
			layout.Add (new Panel { Size = new Size (10, 10), BackgroundColor = Colors.Red }, 2, 1);

			// row 3
			layout.Add (new Panel { Size = new Size (30, 30), BackgroundColor = Colors.Lime }, 0, 2);
			layout.Add (new Panel { Size = new Size (20, 20), BackgroundColor = Colors.Red }, 1, 2);
			layout.Add (new Panel { Size = new Size (10, 10), BackgroundColor = Colors.Lime }, 2, 2);

			// row 4
			layout.Add (new Panel { Size = new Size (10, 10), BackgroundColor = Colors.Red }, 0, 3);
			layout.Add (new Panel { Size = new Size (20, 20), BackgroundColor = Colors.Lime }, 1, 3);
			layout.Add (new Panel { Size = new Size (30, 30), BackgroundColor = Colors.Red }, 2, 3);
			return layout.Container;
		}

		Control MiddlePanel ()
		{
			return new Panel { BackgroundColor = Colors.Blue };
		}

		Control NoPadding ()
		{
			var layout = new TableLayout (new Panel { BackgroundColor = Colors.Blue }, 3, 3);
			layout.Padding = Padding.Empty;
			layout.Spacing = new Size (20, 20);
			// scale first and last column
			layout.SetColumnScale (0);
			layout.SetColumnScale (2);
			// scale first and last row
			layout.SetRowScale (0);
			layout.SetRowScale (2);
			FillTable (layout);
			return layout.Container;
		}



	}
}
