using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class NumericUpDownHandler : MacView<NSView, NumericUpDown>, INumericUpDown
	{
		NSTextField text;
		NSStepper stepper;
		Font font;
		
		class MyView : NSView
		{
			
			
			public override void SetFrameSize (System.Drawing.SizeF newSize)
			{
				base.SetFrameSize (newSize);
				var views = this.Subviews;
				var text = views [0];
				var splitter = views [1];
				var offset = (newSize.Height - text.Frame.Height) / 2;
				text.SetFrameOrigin (new SD.PointF (0, offset));
				text.SetFrameSize (new SD.SizeF (newSize.Width - splitter.Frame.Width, text.Frame.Height));
				offset = (newSize.Height - splitter.Frame.Height) / 2;
				splitter.SetFrameOrigin (new SD.PointF (newSize.Width - splitter.Frame.Width, offset));
			}
		}

		public NumericUpDownHandler ()
		{
			this.Control = new MyView {
				AutoresizesSubviews = false
			};
			text = new NSTextField{
				Bezeled = true,
				Editable = true
			};
			text.Changed += delegate {
				stepper.DoubleValue = text.DoubleValue;
				Widget.OnValueChanged (EventArgs.Empty);
			};
			
			stepper = new NSStepper ();
			stepper.Activated += delegate {
				text.DoubleValue = stepper.DoubleValue;
				Widget.OnValueChanged (EventArgs.Empty);
			};
			MinValue = 0;
			MaxValue = 100;
			Value = 0;
			text.SizeToFit ();
			stepper.SizeToFit ();
			
			/*var stepperWidth = stepper.Frame.Width;
			var width = 100;
			//stepper.SetFrameOrigin (new System.Drawing.PointF(width - stepperWidth, offset));
			//text.Frame = new System.Drawing.RectangleF(0, 0, width - stepperWidth, height);
			*/
			Control.AddSubview (text);
			Control.AddSubview (stepper);
			var naturalHeight = Math.Max (text.Frame.Height, stepper.Frame.Height);
			Control.Frame = new System.Drawing.RectangleF(0, 0, 80, naturalHeight);
		}
		
		protected override void SetNaturalSize ()
		{
			text.SizeToFit ();
			stepper.SizeToFit ();
			var naturalHeight = Math.Max (text.Frame.Height, stepper.Frame.Height);
			Control.SetFrameSize (new System.Drawing.SizeF(80, naturalHeight));
		}
		
		public override void SizeToFit ()
		{
			base.SizeToFit ();
		}
		
		public bool ReadOnly {
			get { return text.Enabled; }
			set { 
				text.Enabled = value;
				stepper.Enabled = value;
			}
		}
		
		public double Value {
			get { return text.DoubleValue; }
			set { 
				text.DoubleValue = value;
				stepper.DoubleValue = value;
			}
		}
		
		public double MinValue {
			get {
				return stepper.MinValue;
			}
			set {
				stepper.MinValue = value;
			}
		}
		
		public double MaxValue {
			get {
				return stepper.MaxValue;
			}
			set {
				stepper.MaxValue = value;
			}
		}
		
		public override bool Enabled {
			get {
				return stepper.Enabled;
			}
			set {
				stepper.Enabled = value;
				text.Enabled = value;
			}
		}

		public Font Font {
			get {
				return font;
			}
			set {
				font = value;
				if (font != null)
					text.Font = font.ControlObject as NSFont;
				else
					text.Font = null;
			}
		}
	}
}
