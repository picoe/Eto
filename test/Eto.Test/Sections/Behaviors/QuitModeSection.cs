namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Quit Mode")]
	public class QuitModeSection : DynamicLayout
	{
		static int windowCount;
		static bool cancelTerminating;
		static bool terminatingHooked;

		public QuitModeSection()
		{
			// static, since it must keep working after the main form (and this section) is gone
			if (!terminatingHooked)
			{
				terminatingHooked = true;
				Application.Instance.Terminating += (sender, e) =>
				{
					e.Cancel |= cancelTerminating;
					if (cancelTerminating)
						Log.Write(null, "Cancelled Terminating");
				};
			}

			var modeList = new EnumRadioButtonList<QuitMode> { Orientation = Orientation.Vertical, SelectedValue = Application.Instance.QuitMode };
			modeList.SelectedValueChanged += (sender, e) =>
			{
				Application.Instance.QuitMode = modeList.SelectedValue;
				// saved so it also applies from startup
				TestApplication.Settings.QuitMode = modeList.SelectedValue;
				TestApplication.Settings.Save();
				Log.Write(this, $"QuitMode: {modeList.SelectedValue}");
			};

			var cancelCheckBox = new CheckBox { Text = "Cancel Terminating", Checked = cancelTerminating };
			cancelCheckBox.CheckedChanged += (sender, e) => cancelTerminating = cancelCheckBox.Checked == true;

			DefaultSpacing = new Size(5, 5);
			Padding = 10;

			BeginCentered();
			Add(new Label { Text = "Windows opened here stay open when you leave this section,\nso you can close the main form and see whether the application quits." });
			Add(modeList);
			Add(cancelCheckBox);
			AddSeparateRow(
				WindowButton("Window", () => new Form()),
				WindowButton("Owned Window", () => new Form { Owner = ParentWindow }),
				WindowButton("Floating Form", () => new FloatingForm()),
				WindowButton("Hidden Window", () => new Form(), hide: true),
				DialogButton()
			);
			EndCentered();
			Add(null);
		}

		Button WindowButton(string text, Func<Form> create, bool hide = false)
		{
			var button = new Button { Text = text };
			button.Click += (sender, e) =>
			{
				var form = create();
				form.Title = $"{text} {++windowCount}";
				form.Size = new Size(300, 150);
				form.Content = WindowContent(form);
				form.Closed += (s, a) => Log.Write(form, $"Closed {form.Title}");
				form.Show();
				if (hide)
				{
					// shown first, so it is open but no longer visible
					form.Visible = false;
					Log.Write(form, $"Hid {form.Title}");
				}
			};
			return button;
		}

		static Button DialogButton()
		{
			var button = new Button { Text = "Dialog" };
			button.Click += async (sender, e) =>
			{
				var dialog = new Dialog { Title = $"Dialog {++windowCount}", Size = new Size(300, 150) };
				// a modal dialog blocks the other windows, so it closes them itself
				var closeForms = new Button { Text = "Close All Forms" };
				closeForms.Click += (s, a) =>
				{
					foreach (var form in Application.Instance.Windows.OfType<Form>().ToList())
						form.Close();
				};
				var close = new Button { Text = "Close" };
				close.Click += (s, a) => dialog.Close();

				var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
				layout.AddCentered(new Label { Text = dialog.Title }, verticalCenter: true);
				layout.AddSeparateRow(null, closeForms, close, null);
				dialog.Content = layout;
				dialog.Closed += (s, a) => Log.Write(dialog, $"Closed {dialog.Title}");

				await dialog.ShowModalAsync();
			};
			return button;
		}

		static Control WindowContent(Form form)
		{
			var close = new Button { Text = "Close" };
			close.Click += (sender, e) => form.Close();
			var minimize = new Button { Text = "Minimize" };
			minimize.Click += (sender, e) => form.WindowState = WindowState.Minimized;
			// the only way out for Explicit once the main form is closed
			var quit = new Button { Text = "Quit" };
			quit.Click += (sender, e) => Application.Instance.Quit();

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.AddCentered(new Label { Text = form.Title }, verticalCenter: true);
			layout.AddSeparateRow(null, close, minimize, quit, null);
			return layout;
		}
	}
}
