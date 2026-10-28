namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(PasswordBox))]
	public class PasswordBoxSection : Panel
	{
		public PasswordBoxSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var passwordBox = Default();

			var backgroundColorPicker = new ColorPicker { AllowAlpha = true };
			backgroundColorPicker.ValueBinding.Bind(passwordBox, c => c.BackgroundColor);

			var textColorPicker = new ColorPicker { AllowAlpha = true };
			textColorPicker.ValueBinding.Bind(passwordBox, c => c.TextColor);

			layout.AddSeparateRow(null, "BackgroundColor", backgroundColorPicker, "TextColor", textColorPicker, null);
			layout.AddRow(new Label { Text = "Default" }, passwordBox);
			layout.AddRow(new Label { Text = "Read Only" }, ReadOnly());
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());

			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		PasswordBox Default()
		{
			var control = new PasswordBox { Text = "Some Text" };
			LogEvents(control);
			return control;
		}

		Control ReadOnly()
		{
			var control = new PasswordBox { Text = "Read only text", ReadOnly = true };
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = Default();
			control.Enabled = false;
			return control;
		}

		void LogEvents(PasswordBox control)
		{
			control.TextChanged += delegate
			{
				Log.Write(control, "TextChanged, Text: {0}", control.Text);
			};
		}
	}
}

