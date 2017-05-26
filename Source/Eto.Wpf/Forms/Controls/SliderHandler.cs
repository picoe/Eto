using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoSlider : swc.Slider, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ??base.MeasureOverride(constraint);
		}
	}

	public class SliderHandler : WpfControl<swc.Slider, Slider, Slider.ICallback>, Slider.IHandler
	{
		public SliderHandler ()
		{
			Control = new EtoSlider
			{
				Handler = this,
				Minimum = 0,
				Maximum = 100,
				TickPlacement = swc.Primitives.TickPlacement.BottomRight
			};
			Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
		}

		protected override sw.Size DefaultSize 
		{
			get {
				if (Orientation == Orientation.Horizontal)
					return new sw.Size(100, double.NaN);
				else
					return new sw.Size(double.NaN, 100);
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public int MaxValue
		{
			get { return (int)Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue
		{
			get { return (int)Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value
		{
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

        public bool SnapToTick
        {
            get { return Control.IsSnapToTickEnabled; }
            set { Control.IsSnapToTickEnabled = value; }
        }

		public int TickFrequency
		{
			get { return (int)Control.TickFrequency; }
			set { Control.TickFrequency = value; }
		}

		public Orientation Orientation
		{
			get
			{
				switch (Control.Orientation) {
					case swc.Orientation.Vertical:
						return Orientation.Vertical;
					case swc.Orientation.Horizontal:
						return Orientation.Horizontal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case Orientation.Vertical:
						Control.Orientation = swc.Orientation.Vertical;
						break;
					case Orientation.Horizontal:
						Control.Orientation = swc.Orientation.Horizontal;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.MouseUpEvent:
					ContainerControl.PreviewMouseDown += (sender, e) =>
					{
						// don't swallow mouse up events for right click and middle click
						e.Handled |= e.ChangedButton != sw.Input.MouseButton.Left;
					};
					base.AttachEvent(id);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
