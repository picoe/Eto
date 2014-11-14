using System;
using Eto.Forms;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Mac.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using nnint = System.nint;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
using nnint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
using nnint = System.Int32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class NumericUpDownHandler : MacView<NSView, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		readonly NSTextField text;
		readonly NSStepper stepper;
		Font font;
		Size? naturalSize;

		public override NSView ContainerControl { get { return Control; } }

		public class EtoTextField : NSTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }
		}

		class EtoNumericUpDownView : MacEventView
		{
			public override void SetFrameSize(CGSize newSize)
			{
				base.SetFrameSize(newSize);
				var views = Subviews;
				var text = views[0];
				var splitter = views[1];
				var offset = (newSize.Height - text.Frame.Height) / 2;
				text.SetFrameOrigin(new CGPoint(0, offset));
				text.SetFrameSize(new CGSize((float)(newSize.Width - splitter.Frame.Width), (float)text.Frame.Height));
				offset = (newSize.Height - splitter.Frame.Height) / 2;
				splitter.SetFrameOrigin(new CGPoint(newSize.Width - splitter.Frame.Width, offset));
			}
		}

		class EtoStepper : NSStepper
		{
			public override bool AcceptsFirstResponder()
			{
				return false;
			}
		}

		public override object EventObject
		{
			get { return text; }
		}

		public NumericUpDownHandler()
		{
			this.Control = new EtoNumericUpDownView
			{
				Handler = this,
				AutoresizesSubviews = false
			};
			text = new EtoTextField
			{
				WeakHandler = new WeakReference(this),
				Bezeled = true,
				Editable = true
			};
			text.Changed += HandleTextChanged;
			
			stepper = new EtoStepper();
			stepper.Activated += HandleStepperActivated;
			MinValue = 0;
			MaxValue = 100;
			Value = 0;
			DecimalPlaces = 0;
			Control.AddSubview(text);
			Control.AddSubview(stepper);
		}

		static void HandleStepperActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)sender).Superview) as NumericUpDownHandler;
			if (handler != null)
			{
				var val = handler.stepper.DoubleValue;
				if (Math.Abs(val) < 1E-10)
				{
					handler.text.IntValue = 0;
				}
				else
				{
					handler.text.DoubleValue = handler.stepper.DoubleValue;
				}
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		static void HandleTextChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)((NSNotification)sender).Object).Superview) as NumericUpDownHandler;
			if (handler != null)
			{
				handler.stepper.DoubleValue = handler.text.DoubleValue;
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			var size = GetNaturalSize(Size.MaxValue);
			Control.Frame = new CGRect(0, 0, size.Width, size.Height);
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Handled)
				return;
			if (e.KeyData == Keys.Down)
			{
				var val = Value;
				var newval = Math.Max(val - 1, MinValue);
				if (newval < val)
				{
					Value = newval;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
				e.Handled = true;
			}
			else if (e.KeyData == Keys.Up)
			{
				var val = Value;
				var newval = Math.Min(val + 1, MaxValue);
				if (newval > val)
				{
					Value = newval;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
				e.Handled = true;
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (naturalSize == null)
			{
				text.SizeToFit();
				stepper.SizeToFit();
				var naturalHeight = Math.Max(text.Frame.Height, stepper.Frame.Height);
				naturalSize = new Size(80, (int)naturalHeight);
			}
			return naturalSize.Value;
		}

		public bool ReadOnly
		{
			get { return text.Enabled; }
			set
			{ 
				text.Enabled = value;
				stepper.Enabled = value;
			}
		}

		public double Value
		{
			get { return text.DoubleValue; }
			set
			{
				if (Math.Abs(value) < 1E-10)
				{
					stepper.IntValue = text.IntValue = 0;
				}
				else
				{
					stepper.DoubleValue = text.DoubleValue = value;
				}
			}
		}

		public double MinValue
		{
			get
			{
				return stepper.MinValue;
			}
			set
			{
				stepper.MinValue = value;
			}
		}

		public double MaxValue
		{
			get
			{
				return stepper.MaxValue;
			}
			set
			{
				stepper.MaxValue = value;
			}
		}

		public override bool Enabled
		{
			get
			{
				return stepper.Enabled;
			}
			set
			{
				stepper.Enabled = value;
				text.Enabled = value;
			}
		}

		public Font Font
		{
			get
			{
				return font ?? (font = new Font(new FontHandler(text.Font)));
			}
			set
			{
				font = value;
				text.Font = font == null ? null : font.ControlObject as NSFont;
				text.SizeToFit();
				LayoutIfNeeded();
			}
		}

		public double Increment
		{
			get { return stepper.Increment; }
			set { stepper.Increment = value; }
		}

		int decimalPlaces;
		public int DecimalPlaces
		{
			get { return decimalPlaces; }
			set
			{
				if (value != decimalPlaces)
				{
					decimalPlaces = value;
					var formatter = new NSNumberFormatter();
					formatter.NumberStyle = NSNumberFormatterStyle.Decimal;
					formatter.MinimumFractionDigits = (nnint)decimalPlaces;
					formatter.MaximumFractionDigits = (nnint)decimalPlaces;
					text.Formatter = formatter;
				}
			}
		}

		public Color TextColor
		{
			get { return text.TextColor.ToEto(); }
			set { text.TextColor = value.ToNSUI(); }
		}

		protected override void SetBackgroundColor(Color? color)
		{
			if (color != null)
				text.BackgroundColor = color.Value.ToNSUI();
		}
	}
}
