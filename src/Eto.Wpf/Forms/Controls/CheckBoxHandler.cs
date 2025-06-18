namespace Eto.Wpf.Forms.Controls
{
	public class CheckBoxHandler : WpfControl<swc.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		readonly swc.Border _border;
		EtoAccessLabel _labelPart;

		EtoAccessLabel LabelPart => _labelPart ??= new EtoAccessLabel();

		public override sw.FrameworkElement ContainerControl => _border;

		public CheckBoxHandler()
		{
			Control = new swc.CheckBox {
				IsThreeState = false,
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center
			};
			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Indeterminate += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			_border = new EtoBorder { Handler = this, Child = Control };
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get { return _border.Background.ToEtoColor(); }
			set { _border.Background = value.ToWpfBrush(Control.Background); }
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool? Checked
		{
			get { return Control.IsChecked; }
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

		public bool ThreeState
		{
			get { return Control.IsThreeState; }
			set { Control.IsThreeState = value; }
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
