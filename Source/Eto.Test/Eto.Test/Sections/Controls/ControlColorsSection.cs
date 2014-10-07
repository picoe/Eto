using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Sections.Behaviors;
using System.Collections.Generic;
using System;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", "Control Colors")]
	public class ControlColorsSection : AllControlsBase
	{
		readonly List<Action<Color>> foregroundUpdates = new List<Action<Color>>();
		readonly List<Action<Color>> backgroundUpdates = new List<Action<Color>>();

		protected override Control CreateOptions()
		{
			var foregroundPicker = new ColorPicker();
			foregroundPicker.ValueChanged += (sender, e) => foregroundUpdates.ForEach(r => r(foregroundPicker.Value));

			var backgroundPicker = new ColorPicker();
			backgroundPicker.ValueChanged += (sender, e) => backgroundUpdates.ForEach(r => r(backgroundPicker.Value));

			var formColorPicker = new ColorPicker { Value = BackgroundColor };
			formColorPicker.ValueChanged += (sender, e) => BackgroundColor = formColorPicker.Value;

			return new TableLayout(
				new TableRow(
					null, 
					new Label { Text = "Text", VerticalAlign = VerticalAlign.Middle },
					foregroundPicker,
					new Label { Text = "Background", VerticalAlign = VerticalAlign.Middle },
					backgroundPicker,
					new Label { Text = "Form", VerticalAlign = VerticalAlign.Middle },
					formColorPicker,
					null
				)
			);
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			//backgroundUpdates.ForEach(r => r(Colors.White));
		}

		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			backgroundUpdates.Add(c => control.BackgroundColor = c);

			var textControl = control as TextControl;
			if (textControl != null)
				foregroundUpdates.Add(c => textControl.TextColor = c);

			var numericUpDown = control as NumericUpDown;
			if (numericUpDown != null)
				foregroundUpdates.Add(c => numericUpDown.TextColor = c);

			var button = control as Button;
			if (button != null)
				foregroundUpdates.Add(c => button.TextColor = c);

			var dateTimePicker = control as DateTimePicker;
			if (dateTimePicker != null)
				foregroundUpdates.Add(c => dateTimePicker.TextColor = c);

			var groupBox = control as GroupBox;
			if (groupBox != null)
				foregroundUpdates.Add(c => groupBox.TextColor = c);

			var listControl = control as ListControl;
			if (listControl != null)
				foregroundUpdates.Add(c => listControl.TextColor = c);
		}
	}
}

