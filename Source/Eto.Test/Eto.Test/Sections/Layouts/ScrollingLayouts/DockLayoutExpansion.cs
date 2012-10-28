using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	public class DockLayoutExpansion : Panel
	{
		public DockLayoutExpansion ()
		{
			var layout = new DynamicLayout (this);

			layout.Add (Default (), yscale: true);
			layout.Add (ExpandedWidth (), yscale: true);
			layout.Add (ExpandedHeight (), yscale: true);
		}

		Control Default ()
		{
			var layout = new DockLayout (new Scrollable ());

			layout.Content = new Label { BackgroundColor = Colors.Red, Text = "Expanded Width/Height (default)" };
			return layout.Container;
		}

		Control ExpandedWidth ()
		{
			var layout = new DockLayout (new Scrollable { ExpandContentHeight = false });

			layout.Content = new Label { BackgroundColor = Colors.Red, Text = "Expanded Width" };
			return layout.Container;
		}

		Control ExpandedHeight ()
		{
			var layout = new DockLayout (new Scrollable { ExpandContentWidth = false });

			layout.Content = new Label { BackgroundColor = Colors.Red, Text = "Expanded Height" };
			return layout.Container;
		}


	}
}

