namespace Eto.Wpf.Forms.Controls
{
	public class RadioButtonHandler : WpfControl<swc.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		swc.Border _border;
		EtoAccessLabel _labelPart;

		EtoAccessLabel LabelPart => _labelPart ??= new EtoAccessLabel();

		public override sw.FrameworkElement ContainerControl => _border;

		public void Create(RadioButton controller)
		{
			Control = new swc.RadioButton
			{
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center,
			};
			if (controller != null)
			{
				var parent = (swc.RadioButton)controller.ControlObject;
				Control.GroupName = parent.GroupName;
			}
			else
				Control.GroupName = Guid.NewGuid().ToString();

			Control.Loaded += Control_Loaded;
			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Click += (sender, e) =>
			{
				// Only raise Click for the radio just checked; WPF routes Click when others uncheck.
				if (Control.IsChecked == true)
					Callback.OnClick(Widget, EventArgs.Empty);
			};

			_border = new EtoBorder { Handler = this, Child = Control };
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			var border = Control.FindChild<swc.Border>("radioButtonBorder");
			if (border != null)
			{
				// ensure the radio button and dot is always round and in the center regardless of DPI
				border.UseLayoutRounding = false;
				border.SnapsToDevicePixels = false;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
		}

		public string Text
		{
			get => _labelPart?.Text;
			set
			{
				if (value == Text || (string.IsNullOrEmpty(value) && _labelPart == null))
					return;
				LabelPart.Text = value;
				Control.Content = LabelPart;
			}
		}

		public override Color BackgroundColor
		{
			get { return _border.Background.ToEtoColor(); }
			set { _border.Background = value.ToWpfBrush(_border.Background); }
		}

		public bool UseMnemonic
		{
			get => _labelPart?.UseMnemonic ?? true;
			set => LabelPart.UseMnemonic = value;
		}

		public bool AlwaysShowMnemonic
		{
			get => _labelPart?.AlwaysShowMnemonic ?? false;
			set => LabelPart.AlwaysShowMnemonic = value;
		}

		public bool EnableMnemonic
		{
			get => _labelPart?.EnableMnemonic ?? true;
			set => LabelPart.EnableMnemonic = value;
		}
	}
}
