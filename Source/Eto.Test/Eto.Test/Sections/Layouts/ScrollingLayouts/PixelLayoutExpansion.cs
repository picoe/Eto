using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	public class PixelLayoutExpansion : Panel
	{
		Scrollable defaultScrollable;
		public PixelLayoutExpansion ()
		{
			var layout = new DynamicLayout (this);

			defaultScrollable = new Scrollable();
			layout.AddSeparateRow (null, ExpandContentWidth (), ExpandContentHeight (), null);
			layout.Add (Default (), yscale: true);
			layout.Add (ExpandedWidth (), yscale: true);
			layout.Add (ExpandedHeight (), yscale: true);
		}

		Control Default ()
		{
			var layout = new PixelLayout (defaultScrollable);

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Width/Height (default)" }, 50, 50);
			return layout.Container;
		}

		Control ExpandContentWidth ()
		{
			var control = new CheckBox { Text = "ExpandContentWidth" };
			control.Bind ("Checked", defaultScrollable, "ExpandContentWidth");
			return control;
		}

		Control ExpandContentHeight ()
		{
			var control = new CheckBox { Text = "ExpandContentHeight" };
			control.Bind ("Checked", defaultScrollable, "ExpandContentHeight");
			return control;
		}

		Control ExpandedWidth ()
		{
			var layout = new PixelLayout (new Scrollable { ExpandContentHeight = false });

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Width" }, 50, 50);
			return layout.Container;
		}

		Control ExpandedHeight ()
		{
			var layout = new PixelLayout (new Scrollable { ExpandContentWidth = false });

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Height" }, 50, 50);
			return layout.Container;
		}


	}
}

