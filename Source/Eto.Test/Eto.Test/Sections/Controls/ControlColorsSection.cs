using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Sections.Behaviors;
using System.Collections.Generic;
using System;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", "Control Font & Colors")]
	public class ControlColorsSection : AllControlsBase
	{
		readonly List<Action<Font>> fontUpdates = new List<Action<Font>>();
		readonly List<Action<Color>> foregroundUpdates = new List<Action<Color>>();
		readonly List<Action<Color>> backgroundUpdates = new List<Action<Color>>();

		protected override Control CreateOptions()
		{
			var foregroundPicker = new ColorPicker();
			foregroundPicker.ValueChanged += (sender, e) =>
			{
				foreach (var update in foregroundUpdates)
					update(foregroundPicker.Value);
			};

			var backgroundPicker = new ColorPicker();
			backgroundPicker.ValueChanged += (sender, e) =>
			{
				foreach (var update in backgroundUpdates)
					update(backgroundPicker.Value);
			};

			var formColorPicker = new ColorPicker { Value = BackgroundColor };
			formColorPicker.ValueChanged += (sender, e) => BackgroundColor = formColorPicker.Value;

			var fontPicker = new Button { Text = "Pick Font" };
			fontPicker.Click += (sender, e) => {
				var dlg = new FontDialog();
				dlg.FontChanged += (sender2, e2) => {
					var font = dlg.Font;
					foreach (var update in fontUpdates)
						update(font);
				};
				dlg.ShowDialog(this);
			};

			return new TableLayout(
				new TableRow(
					null, 
					new Label { Text = "Text", VerticalAlign = VerticalAlign.Middle },
					foregroundPicker,
					new Label { Text = "Background", VerticalAlign = VerticalAlign.Middle },
					backgroundPicker,
					new Label { Text = "Form", VerticalAlign = VerticalAlign.Middle },
					formColorPicker,
					fontPicker,
					null
				)
			);
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
			{
				foregroundUpdates.Add(c => groupBox.TextColor = c);
				fontUpdates.Add(c => groupBox.Font = c);
			}

			var listControl = control as ListControl;
			if (listControl != null)
				foregroundUpdates.Add(c => listControl.TextColor = c);

			var commonControl = control as CommonControl;
			if (commonControl != null)
				fontUpdates.Add(c => commonControl.Font = c);

		}
	}
}

