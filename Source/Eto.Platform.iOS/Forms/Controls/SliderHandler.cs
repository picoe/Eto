using System;
using MonoTouch.UIKit;
using Eto.Forms;
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SliderHandler : iosControl<UISlider, Slider>, ISlider
	{
		SliderOrientation orientation;

		public override UISlider CreateControl ()
		{
			return new UISlider ();
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			MaxValue = 100;
		}

		public int MaxValue {
			get { return (int)Control.MaxValue; }
			set { Control.MaxValue = value; }
		}

		public int MinValue {
			get { return (int)Control.MinValue; }
			set { Control.MinValue = value; }
		}

		public int Value {
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

		public int TickFrequency {
			get;
			set;
		}

		public bool SnapToTick {
			get;
			set;
		}

		public SliderOrientation Orientation {
			get { return orientation; }
			set {
				if (orientation != value) {
					orientation = value;
					switch (value) {
					case SliderOrientation.Horizontal:
						Control.Transform = new CGAffineTransform();
						break;
					case SliderOrientation.Vertical:
						Control.Transform = CGAffineTransform.MakeRotation ((float)Math.PI * -0.5f);
						break;
					default:
						throw new NotSupportedException ();
					}
				}
			}
		}
	}
}

