namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(LinkButton))]
	public class LinkButtonSection : Scrollable
	{
		public LinkButtonSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddAutoSized(NormalButton(), centered: true);
			layout.AddAutoSized(LongerButton(), centered: true);
			layout.AddAutoSized(ColourButton(), centered: true);
			layout.AddAutoSized(DisabledButton(), centered: true);
			layout.AddAutoSized(DisabledButtonWithColor(), centered: true);
			layout.Add(StretchedButton());
			layout.Add(WrapAndTrimButton());
			layout.Add(null);

			Content = layout;
		}

		const string LongUrl = "https://www.example.com/a/rather/long/path/that/will/not/fit/in/a/narrow/window";

		/// <summary>
		/// A link showing something long that does not need reading in full, in a host of a set width so
		/// Wrap and Trimming have something to work against.
		/// </summary>
		Control WrapAndTrimButton()
		{
			var control = new LinkButton { Text = LongUrl };
			LogEvents(control);

			// The background shows the room the link has been given, so it is obvious whether the text is
			// being fitted to it or spilling out of it.
			var host = new Panel
			{
				Width = 250,
				BackgroundColor = Colors.LightGrey,
				Content = control
			};

			var wrapDropDown = new EnumDropDown<WrapMode>();
			wrapDropDown.SelectedValueBinding.Bind(control, c => c.Wrap);

			var trimmingDropDown = new EnumDropDown<TextTrimming>();
			trimmingDropDown.SelectedValueBinding.Bind(control, c => c.Trimming);

			var textBox = new TextBox { Text = control.Text, Width = 400 };
			textBox.TextChanged += (sender, e) => control.Text = textBox.Text;

			return new StackLayout
			{
				// Left, not Stretch: stretching the host would throw away the size set on it.
				HorizontalContentAlignment = HorizontalAlignment.Left,
				Spacing = 5,
				Items =
				{
					TableLayout.Horizontal(5, "Wrap:", wrapDropDown, "Trimming:", trimmingDropDown),
					SizeSlider("Width:", 40, 600, host.Width, value => host.Width = value),
					SizeSlider("Height:", 0, 120, 0, value => host.Height = value == 0 ? -1 : value, autoAtMinimum: true),
					TableLayout.Horizontal(5, "Text:", textBox),
					host
				}
			};
		}

		/// <summary>
		/// A labelled slider for one dimension of the host, showing the value beside it.
		/// </summary>
		/// <param name="autoAtMinimum">True when the lowest value means "no limit" rather than zero.</param>
		static Control SizeSlider(string label, int min, int max, int initial, Action<int> apply, bool autoAtMinimum = false)
		{
			var slider = new Slider { MinValue = min, MaxValue = max, Value = initial, Width = 200 };
			var valueLabel = new Label { Width = 50 };

			void Update()
			{
				apply(slider.Value);
				valueLabel.Text = autoAtMinimum && slider.Value == min ? "auto" : $"{slider.Value}px";
			}

			slider.ValueChanged += (sender, e) => Update();
			Update();

			return TableLayout.Horizontal(5, label, slider, valueLabel);
		}

		Control NormalButton()
		{
			var control = new LinkButton { Text = "Click Me" };
			LogEvents(control);
			return control;
		}

		Control StretchedButton()
		{
			var control = new LinkButton { Text = "A stretched button" };
			LogEvents(control);
			return control;
		}

		Control LongerButton()
		{
			var control = new LinkButton { Text = "This is a long(er) button title" };
			LogEvents(control);
			return control;
		}

		Control ColourButton()
		{
			var control = new LinkButton { Text = "Button with Color", TextColor = Colors.Lime };
			LogEvents(control);
			return control;
		}

		Control DisabledButton()
		{
			var control = new LinkButton { Text = "Disabled Button", Enabled = false };
			LogEvents(control);
			return control;
		}

		Control DisabledButtonWithColor()
		{
			var control = new LinkButton { Text = "Disabled Button with color", DisabledTextColor = Colors.Yellow, Enabled = false };
			LogEvents(control);
			return control;
		}

		void LogEvents(LinkButton button)
		{
			button.Click += delegate
			{
				Log.Write(button, "Click");
			};
		}
	}
}

