using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	[Section("TableLayout", "Scaling")]
	public class ScalingSection : Scrollable
	{
		public ScalingSection()
		{
			TableLayout tableLayout;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			var size = new Size(-1, 100);

			tableLayout = new TableLayout(1, 1) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.Add(new Label { Text = "1x1, should scale to fill entire region with 5px padding around border", BackgroundColor = Colors.Red }, 0, 0, false, false);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(2, 2) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.Add(new Label { Text = "2x2, should scale with extra space on top && left", BackgroundColor = Colors.Red }, 1, 1, false, false);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(2, 2) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.Add(new Label { Text = "2x2, should scale with extra space on bottom && right", BackgroundColor = Colors.Red }, 0, 0, true, true);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(3, 3) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.Add(new Label { Text = "3x3, should scale with extra space all around (10px)", BackgroundColor = Colors.Red }, 1, 1, true, true);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(2, 2) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.Add(new Label { Text = "2x2, should not scale and be top left", BackgroundColor = Colors.Red }, 0, 0, false, false);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(2, 2) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.SetColumnScale(0);
			tableLayout.SetRowScale(0);
			tableLayout.Add(new Label { Text = "2x2, should not scale and be bottom-right", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(3, 3) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.SetColumnScale(0);
			tableLayout.SetRowScale(0);
			tableLayout.Add(new Label { Text = "3x3, should not scale and be bottom-right", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add(tableLayout, yscale: true);

			tableLayout = new TableLayout(3, 3) { BackgroundColor = Colors.Blue, Size = size, Spacing = new Size(5, 5), Padding = new Padding(5) };
			tableLayout.SetColumnScale(0);
			tableLayout.SetColumnScale(2);
			tableLayout.SetRowScale(0);
			tableLayout.SetRowScale(2);
			tableLayout.Add(new Label { Text = "2x2, should not scale and be centered", BackgroundColor = Colors.Red }, 1, 1);
			layout.Add(tableLayout, yscale: true);

			Content = layout;
		}
	}
}
