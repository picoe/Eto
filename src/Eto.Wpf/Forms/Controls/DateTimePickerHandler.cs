using Eto.Wpf.Drawing;
using Eto.Wpf.CustomControls;

namespace Eto.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfFrameworkElement<EtoBorder, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		DateTimePickerControl _picker;
		DateTime? last;

		DateTimePickerMode mode;

		protected override sw.Size DefaultSize => new sw.Size(mode == DateTimePickerMode.DateTime ? 180 : 120, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public DateTimePickerHandler()
		{
			_picker = new DateTimePickerControl();
			_picker.ValueChanged += Picker_ValueChanged;

			Control = new EtoBorder { Handler = this, Focusable = false };
			Control.Child = _picker;
			Mode = DateTimePickerMode.Date;
		}

		public bool ShowBorder
		{
			get { return !Control.BorderThickness.ToEto().IsZero; }
			set { Control.BorderThickness = value ? new sw.Thickness(1) : new sw.Thickness(0); }
		}

		void Picker_ValueChanged(object sender, EventArgs e)
		{
			var val = Value;
			if (last != val && (last == null || val == null || Math.Abs((last.Value - val.Value).TotalSeconds) >= 1))
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				last = val;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public DateTime? Value
		{
			get { return _picker.Value; }
			set { _picker.Value = value; }
		}

		public DateTime MinDate
		{
			get { return _picker.Minimum ?? DateTime.MinValue; }
			set { _picker.Minimum = value == DateTime.MinValue ? null : (DateTime?)value; }
		}

		public DateTime MaxDate
		{
			get { return _picker.Maximum ?? DateTime.MaxValue; }
			set { _picker.Maximum = value == DateTime.MaxValue ? null : (DateTime?)value; }
		}

		public DateTimePickerMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				_picker.Mode = value;
				SetSize();
			}
		}

		public override Color BackgroundColor
		{
			get { return _picker.Background.ToEtoColor(); }
			set { _picker.Background = value.ToWpfBrush(_picker.Background); }
		}

		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}

		static readonly object FontKey = new object();

		public Font Font
		{
			get { return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(_picker.FocusableControl))); }
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					Widget.Properties[FontKey] = value;
					_picker.FocusableControl.SetEtoFont(value, SetDecorations);
				}
			}
		}

		public Color TextColor
		{
			get { return _picker.Foreground.ToEtoColor(); }
			set { _picker.Foreground = value.ToWpfBrush(_picker.Foreground); }
		}
	}
}
