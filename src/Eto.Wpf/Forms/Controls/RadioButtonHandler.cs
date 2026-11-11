namespace Eto.Wpf.Forms.Controls
{
	public class EtoRadioButton : swc.RadioButton, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}
	
	public class RadioButtonHandler : WpfControl<swc.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		swc.Border _border;
		EtoAccessLabel _labelPart;

		EtoAccessLabel LabelPart => _labelPart ??= new EtoAccessLabel();

		public override sw.FrameworkElement ContainerControl => _border;

		public RadioButtonHandler()
		{
			Control = new EtoRadioButton
			{
				Handler = this,
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center,
				GroupName = Guid.NewGuid().ToString()
			};

			Control.Loaded += Control_Loaded;
			Control.Checked += Control_CheckedChanged;
			Control.Unchecked += Control_CheckedChanged;
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);

			_border = new EtoBorder { Handler = this, Child = Control };
		}

		bool _settingGroup;
		RadioButton _controller;

		void Control_CheckedChanged(object sender, sw.RoutedEventArgs e)
		{
			if (!_settingGroup)
				Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public RadioButton Controller
		{
			get => _controller;
			set
			{
				_controller = value;
				var groupName = value != null ? ((swc.RadioButton)value.ControlObject).GroupName : Guid.NewGuid().ToString();
				if (Control.GroupName == groupName)
					return;

				Control.GroupName = groupName;
				if (Control.IsChecked == true)
				{
					// WPF only unchecks the rest of the group when a button becomes checked
					_settingGroup = true;
					try
					{
						Control.IsChecked = false;
						Control.IsChecked = true;
					}
					finally
					{
						_settingGroup = false;
					}
				}
			}
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
