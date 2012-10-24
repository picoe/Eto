using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	public class ScalingSection : Scrollable
	{
		public ScalingSection ()
		{
			TableLayout tableLayout;

			var layout = new DynamicLayout (this);
			var size = new Size (-1, 100);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 1, 1);
			tableLayout.Add (new Label { Text = "1x1, should scale to fill entire region (minus padding)", BackgroundColor = Colors.Red }, 0, 0, false, false);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 2, 2);
			tableLayout.Add (new Label { Text = "2x2, should scale", BackgroundColor = Colors.Red }, 1, 1, false, false);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 2, 2);
			tableLayout.Add (new Label { Text = "2x2, should scale", BackgroundColor = Colors.Red }, 0, 0, true, true);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 3, 3);
			tableLayout.Add (new Label { Text = "3x3, should scale", BackgroundColor = Colors.Red }, 1, 1, true, true);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 2, 2);
			tableLayout.Add (new Label { Text = "2x2, should not scale and be top left", BackgroundColor = Colors.Red }, 0, 0, false, false);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 2, 2);
			tableLayout.SetColumnScale (0);
			tableLayout.SetRowScale (0);
			tableLayout.Add (new Label { Text = "2x2, should not scale and be bottom-right", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 3, 3);
			tableLayout.SetColumnScale (0);
			tableLayout.SetRowScale (0);
			tableLayout.Add (new Label { Text = "2x2, should not scale and be bottom-right", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add (tableLayout.Container, yscale: true);

			tableLayout = new TableLayout (new Panel { BackgroundColor = Colors.Blue, Size = size }, 3, 3);
			tableLayout.SetColumnScale (0);
			tableLayout.SetColumnScale (2);
			tableLayout.SetRowScale (0);
			tableLayout.SetRowScale (2);
			tableLayout.Add (new Label { Text = "2x2, should not scale and be centered", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add (tableLayout.Container, yscale: true);
		}
	}
}
