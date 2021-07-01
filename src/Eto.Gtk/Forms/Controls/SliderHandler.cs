using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class SliderHandler : GtkControl<Gtk.EventBox, Slider, Slider.ICallback>, Slider.IHandler
	{
		double min;
		double max = 100;
		double tick = 1;
		bool snapToTick;
		Gtk.Scale scale;

		public SliderHandler()
		{
			this.Control = new Gtk.EventBox();
			//Control.VisibleWindow = false;
			scale = new Gtk.HScale(min, max, SnapToTick ? tick : 0.000000001D);
			this.Control.Child = scale;
		}

		protected override void Initialize()
		{
			base.Initialize();
			scale.ValueChanged += Connector.HandleScaleValueChanged;
		}

		protected new SliderConnector Connector { get { return (SliderConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new SliderConnector();
		}

		protected class SliderConnector : GtkControlConnector
		{
			double? lastValue;

			public new SliderHandler Handler { get { return (SliderHandler)base.Handler; } }

			public void HandleScaleValueChanged(object sender, EventArgs e)
			{
				var scale = Handler.scale;
				var tick = Handler.tick;
				var value = scale.Value;
				if (tick > 0)
				{
					var offset = value % tick;
					if (Handler.SnapToTick && offset != 0)
					{
						// snap to the tick
						if (offset > tick / 2)
							scale.Value = value - offset + tick;
						else
							scale.Value -= offset;
						return;
					}
				}

				if (lastValue == null || lastValue.Value != value)
				{
					Handler.Callback.OnValueChanged(Handler.Widget, EventArgs.Empty);
					lastValue = value;
				}
			}
		}

		public double MaxValue
		{
			get { return max; }
			set
			{
				max = value;
				scale.SetRange(min, max);
			}
		}

		public double MinValue
		{
			get { return min; }
			set
			{
				min = value;
				scale.SetRange(min, max);
			}
		}

		public double Value
		{
			get { return scale.Value; }
			set { scale.Value = value; }
		}

		public bool SnapToTick
		{
			get
			{
				return snapToTick;
			}
			set
			{
				snapToTick = value;
				UpdateScale(Orientation);
			}
		}

		public double TickFrequency
		{
			get
			{
				return tick;
			}
			set
			{
				tick = value;
				// TODO: Only supported from GTK 2.16
			}
		}

		public Orientation Orientation
		{
			get
			{
				return (scale is Gtk.HScale) ? Orientation.Horizontal : Orientation.Vertical;
			}
			set
			{
				if (Orientation != value)
				{
					UpdateScale(value);
				}
			}
		}

		protected void UpdateScale(Orientation value)
		{
			scale.ValueChanged -= Connector.HandleScaleValueChanged;
			Control.Remove(scale);
#if !GTKCORE
			scale.Destroy();
#endif
			scale.Dispose();
			if (value == Orientation.Horizontal)
				scale = new Gtk.HScale(min, max, SnapToTick ? tick : 0.000000001D);
			else
				scale = new Gtk.VScale(min, max, SnapToTick ? tick : 0.000000001D);
			scale.ValueChanged += Connector.HandleScaleValueChanged;
			Control.Child = scale;
			scale.ShowAll();
		}
	}
}

