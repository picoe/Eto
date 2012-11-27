using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	public class DockLayoutExpansion : Panel
	{
		Scrollable defaultScrollable;
		public DockLayoutExpansion ()
		{
			var layout = new DynamicLayout (this);

			defaultScrollable = new Scrollable ();
			layout.AddSeparateRow (null, ExpandContentWidth (), ExpandContentHeight (), null);
			layout.Add (Default (), yscale: true);
			layout.Add (ExpandedWidth (), yscale: true);
			layout.Add (ExpandedHeight (), yscale: true);
		}

		Control Default ()
		{
			var layout = new DockLayout (defaultScrollable);

			layout.Content = new Label { BackgroundColor = Colors.Red, Text = "Expanded Width/Height (default)" };
			return layout.Container;
		}

		Control ExpandContentWidth ()
		{
			var control = new CheckBox { Text = "ExpandContentWidth" };
			control.Bind (r => r.Checked, defaultScrollable, s => s.ExpandContentWidth);
			return control;
		}

		Control ExpandContentHeight ()
		{
			var control = new CheckBox { Text = "ExpandContentHeight" };
			control.Bind (r => r.Checked, defaultScrollable, s => s.ExpandContentHeight);
			return control;
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

