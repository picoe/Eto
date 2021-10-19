using System;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class SliderHandler : WindowsControl<swf.TrackBar, Slider, Slider.ICallback>, Slider.IHandler
	{
		int? lastValue;

		class EtoTrackBar : swf.TrackBar
		{
			protected override void OnCreateControl()
			{
				SetStyle(swf.ControlStyles.SupportsTransparentBackColor, true);
				if (Parent != null)
					BackColor = Parent.BackColor;

				base.OnCreateControl();
			}
		}

		public SliderHandler()
		{
			this.Control = new EtoTrackBar
			{
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

		public double MaxValue
		{
			get { return (double)Control.Maximum; }
			set { Control.Maximum = (int)value; }
		}

		public double MinValue
		{
			get { return (double)Control.Minimum; }
			set { Control.Minimum = (int)value; }
		}

		public double Value
		{
			get { return (double)Control.Value; }
			set { Control.Value = (int)value; }
		}

		public bool SnapToTick { get; set; }

		public double TickFrequency
		{
			get { return (double)Control.TickFrequency; }
			set
			{
				Control.TickFrequency = (int)value;
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

