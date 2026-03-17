namespace Eto.GirCore.Forms.Controls
{
	public class SliderHandler : GirControl<Gtk.Scale, Slider, Slider.ICallback>, Slider.IHandler
	{
		int minValue;
		int maxValue = 100;
		int tickFrequency = 1;
		bool suppressValueChanged;

		public SliderHandler()
		{
			Control = Gtk.Scale.NewWithRange(Gtk.Orientation.Horizontal, 0, 100, 1);
			Control.DrawValue = false;
			Control.ValuePos = Gtk.PositionType.Bottom;
			Control.Adjustment!.OnValueChanged += delegate { HandleAdjustmentValueChanged(); };
		}

		void HandleAdjustmentValueChanged()
		{
			if (suppressValueChanged)
				return;

			var value = (int)Math.Round(Control.GetValue());
			if (tickFrequency > 0 && SnapToTick)
			{
				var offset = (value - minValue) % tickFrequency;
				if (offset != 0)
				{
					var snapped = offset > tickFrequency / 2
						? value - offset + tickFrequency
						: value - offset;
					Value = snapped;
					return;
				}
			}

			Callback.OnValueChanged(Widget, EventArgs.Empty);
		}

		public int MaxValue
		{
			get => maxValue;
			set
			{
				maxValue = value;
				Control.SetRange(minValue, maxValue);
			}
		}

		public int MinValue
		{
			get => minValue;
			set
			{
				minValue = value;
				Control.SetRange(minValue, maxValue);
			}
		}

		public int Value
		{
			get => (int)Math.Round(Control.GetValue());
			set
			{
				suppressValueChanged = true;
				Control.SetValue(value);
				suppressValueChanged = false;
			}
		}

		public int TickFrequency
		{
			get => tickFrequency;
			set
			{
				tickFrequency = Math.Max(1, value);
				Control.SetIncrements(tickFrequency, tickFrequency);
			}
		}

		public bool SnapToTick { get; set; }

		public Orientation Orientation
		{
			get => Control.GetOrientation() == Gtk.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
			set
			{
				Control.SetOrientation(value == Orientation.Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical);
				Control.ValuePos = value == Orientation.Horizontal ? Gtk.PositionType.Bottom : Gtk.PositionType.Left;
			}
		}
	}
}
