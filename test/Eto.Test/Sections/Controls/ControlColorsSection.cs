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
			var foregroundPicker = new ColorPicker { AllowAlpha = true };
			foregroundPicker.ValueChanged += (sender, e) =>
			{
				foreach (var update in foregroundUpdates)
					update(foregroundPicker.Value);
			};

			var backgroundPicker = new ColorPicker { AllowAlpha = false }; // alpha not supported for all controls
			backgroundPicker.ValueChanged += (sender, e) =>
			{
				foreach (var update in backgroundUpdates)
					update(backgroundPicker.Value);
			};

			var formColorPicker = new ColorPicker { Value = BackgroundColor, AllowAlpha = true };
			formColorPicker.ValueChanged += (sender, e) => BackgroundColor = formColorPicker.Value;

			var fontPicker = new FontPicker { Value = SystemFonts.Default() };
			fontPicker.ValueChanged += (sender, e) =>
			{
				var font = fontPicker.Value;
				foreach (var update in fontUpdates)
					update(font);
			};

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items = {
					null,
					new Label { Text = "Text", VerticalAlignment = VerticalAlignment.Center },
					foregroundPicker,
					new Label { Text = "Background", VerticalAlignment = VerticalAlignment.Center },
					backgroundPicker,
					new Label { Text = "Form", VerticalAlignment = VerticalAlignment.Center },
					formColorPicker,
					fontPicker,
					null
				}
			};
		}

		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			backgroundUpdates.Add(c => control.BackgroundColor = c);

			var textControl = control as TextControl;
			if (textControl != null)
				foregroundUpdates.Add(c => textControl.TextColor = c);

			var numericStepper = control as NumericStepper;
			if (numericStepper != null)
				foregroundUpdates.Add(c => numericStepper.TextColor = c);

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

