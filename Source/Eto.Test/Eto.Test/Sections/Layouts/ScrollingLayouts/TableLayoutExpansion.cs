using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	public class TableLayoutExpansion : Panel
	{
		Scrollable defaultScrollable;
		public TableLayoutExpansion ()
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
			var layout = new DynamicLayout (defaultScrollable);

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Width/Height (default)" });
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
			var layout = new DynamicLayout (new Scrollable { ExpandContentHeight = false });

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Width" });
			return layout.Container;
		}

		Control ExpandedHeight ()
		{
			var layout = new DynamicLayout (new Scrollable { ExpandContentWidth = false });

			layout.Add (new Label { BackgroundColor = Colors.Red, Text = "Expanded Height" });
			return layout.Container;
		}


	}
}

