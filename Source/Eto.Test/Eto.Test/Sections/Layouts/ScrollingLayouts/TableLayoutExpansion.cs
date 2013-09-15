using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	public class TableLayoutExpansion : Panel
	{
		Scrollable defaultScrollable;

		public TableLayoutExpansion()
		{
			var layout = new DynamicLayout();

			defaultScrollable = new Scrollable();
			layout.AddSeparateRow(null, ExpandContentWidth(), ExpandContentHeight(), null);
			layout.Add(Default(), yscale: true);
			layout.Add(ExpandedWidth(), yscale: true);
			layout.Add(ExpandedHeight(), yscale: true);

			Content = layout;
		}

		Control Default()
		{
			var layout = new DynamicLayout();

			layout.Add(new Label { BackgroundColor = Colors.Red, Text = "Expanded Width/Height (default)" });

			defaultScrollable.Content = layout;
			return defaultScrollable;
		}

		Control ExpandContentWidth()
		{
			var control = new CheckBox { Text = "ExpandContentWidth" };
			control.Bind(r => r.Checked, defaultScrollable, s => s.ExpandContentWidth);
			return control;
		}

		Control ExpandContentHeight()
		{
			var control = new CheckBox { Text = "ExpandContentHeight" };
			control.Bind(r => r.Checked, defaultScrollable, s => s.ExpandContentHeight);
			return control;
		}

		Control ExpandedWidth()
		{
			var layout = new DynamicLayout();

			layout.Add(new Label { BackgroundColor = Colors.Red, Text = "Expanded Width" });
			return new Scrollable { ExpandContentHeight = false, Content = layout };
		}

		Control ExpandedHeight()
		{
			var layout = new DynamicLayout();

			layout.Add(new Label { BackgroundColor = Colors.Red, Text = "Expanded Height" });
			return new Scrollable { ExpandContentWidth = false, Content = layout };
		}
	}
}

