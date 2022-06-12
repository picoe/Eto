using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(FontPicker))]
	public class FontPickerSection : Panel
	{
		public FontPickerSection()
		{
			var layout = new DynamicLayout();
			layout.DefaultPadding = new Padding(10);
			layout.DefaultSpacing = new Size(4, 4);
			layout.BeginVertical();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Default: ", VerticalAlignment = VerticalAlignment.Center });
			var fontpicker1 = new FontPicker();
			LogEvents(fontpicker1);
			layout.Add(fontpicker1, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Initial Value: ", VerticalAlignment = VerticalAlignment.Center });
			var fontpicker2 = new FontPicker(SystemFonts.User());
			LogEvents(fontpicker2);
			layout.Add(fontpicker2, true, false);
			layout.EndHorizontal();

			layout.Add(null, false, true);

			layout.EndVertical();
			Content = layout;
		}

		void LogEvents(FontPicker control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "FontChanged, Font: " + control.Value);
			};
		}
	}
}
