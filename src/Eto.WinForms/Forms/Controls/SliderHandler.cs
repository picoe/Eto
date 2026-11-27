namespace Eto.WinForms.Forms.Controls
{
	public class SliderHandler : WindowsControl<swf.TrackBar, Slider, Slider.ICallback>, Slider.IHandler
	{
		int? lastValue;

		class EtoTrackBar : swf.TrackBar
		{
			public SliderHandler Handler { get; set; }

			protected override void OnCreateControl()
			{
				SetStyle(swf.ControlStyles.SupportsTransparentBackColor, true);
				if (Handler?.BackgroundColorSet != true)
				{
					// a see-through trackbar draws in the light style in dark mode, so use the nearest solid color
					for (var parent = Parent; parent != null; parent = parent.Parent)
					{
						if (parent.BackColor.A == 255)
						{
							BackColor = parent.BackColor;
							break;
						}
					}
				}

				base.OnCreateControl();
			}
		}

		public SliderHandler()
		{
			this.Control = new EtoTrackBar
			{
				Handler = this,
				TickStyle = System.Windows.Forms.TickStyle.BottomRight,
				Maximum = 100,
				AutoSize = true
			};
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Control.ValueChanged += HandleScaleValueChanged;
		}

		void HandleScaleValueChanged(object sender, EventArgs e)
		{
			var value = Control.Value;
			var tick = Control.TickFrequency;
			var offset = value % tick;
			if (SnapToTick && offset != 0)
			{
				if (offset > tick / 2)
					Control.Value = value - offset + tick;
				else
					Control.Value -= offset;
			}
			else if (lastValue == null || lastValue.Value != value)
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				lastValue = value;
			}
		}

		public int MaxValue
		{
			get { return Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue
		{
			get { return Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public bool SnapToTick { get; set; }

		public int TickFrequency
		{
			get { return Control.TickFrequency; }
			set
			{
				Control.TickFrequency = value;
			}
		}

		public Orientation Orientation
		{
			get { return Control.Orientation == swf.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical; }
			set { Control.Orientation = value == Orientation.Horizontal ? swf.Orientation.Horizontal : swf.Orientation.Vertical; }
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}

