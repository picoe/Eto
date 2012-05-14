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
		Size? naturalSize;
		
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
		
		public override object EventObject {
			get {
				return text;
			}
		}
		
		public class EtoUpDownTextField : NSTextField, IMacControl
		{
			public object Handler { get; set; }
		}

		public NumericUpDownHandler ()
		{
			this.Control = new MyView {
				AutoresizesSubviews = false
			};
			text = new EtoUpDownTextField {
				Handler = this,
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
			
			Control.AddSubview (text);
			Control.AddSubview (stepper);
			var naturalSize = GetNaturalSize ();
			Control.Frame = new System.Drawing.RectangleF (0, 0, naturalSize.Width, naturalSize.Height);
		}
		
		protected override Size GetNaturalSize ()
		{
			if (naturalSize == null) {
				text.SizeToFit ();
				stepper.SizeToFit ();
				var naturalHeight = Math.Max (text.Frame.Height, stepper.Frame.Height);
				naturalSize = new Size (80, (int)naturalHeight);
			}
			return naturalSize.Value;
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
				text.SizeToFit ();
				LayoutIfNeeded();
			}
		}
	}
}
