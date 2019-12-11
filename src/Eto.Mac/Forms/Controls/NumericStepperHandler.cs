using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if XAMMAC
using nnint = System.Int32;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class NumericStepperHandler : MacView<NumericStepperHandler.EtoNumericStepperView, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		Size? naturalSize;

		public override NSView ContainerControl => Control;

		public override NSView FocusControl => Control.TextField;

		public NSTextField TextField => Control.TextField;

		public NSStepper Stepper => Control.Stepper;

		public class EtoTextField : NSTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }
		}

		public class EtoNumericStepperView : NSView, IMacControl
		{
			public NSTextField TextField { get; private set; }
			public NSStepper Stepper { get; private set; }

			public WeakReference WeakHandler { get; set; }

			public override void SetFrameSize(CGSize newSize)
			{
				var spacing = 3;

				base.SetFrameSize(newSize);
				var views = Subviews;
				var text = views[0];
				var stepper = views[1];

				var stepperSize = stepper.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, stepper.FittingSize)).Size;
				stepperSize.Height = (nfloat)Math.Min(newSize.Height, stepperSize.Height);

				var stepperFrame = new CGRect(); 
				stepperFrame.Size = stepperSize;
				stepperFrame.X = newSize.Width - stepperFrame.Width;
				stepperFrame.Y = (nfloat)Math.Truncate((newSize.Height - stepperSize.Height) / 2);
				stepper.Frame = stepper.GetFrameForAlignmentRect(stepperFrame);

				var textFrame = new CGRect();
				textFrame.Height = newSize.Height;
				textFrame.Width = newSize.Width - stepperFrame.Width - spacing;
				text.Frame = textFrame;


				var h = WeakHandler?.Target as IMacViewHandler;
				if (h == null)
					return;

				h.OnSizeChanged(EventArgs.Empty);
				h.Callback.OnSizeChanged(h.Widget, EventArgs.Empty);
			}

			public EtoNumericStepperView(NumericStepperHandler handler)
			{
				AutoresizesSubviews = false;
				TextField = new EtoTextField
				{
					WeakHandler = new WeakReference(handler),
					Bezeled = true,
					Editable = true,
					Formatter = DefaultFormatter
				};
				TextField.Changed += HandleTextChanged;

				Stepper = new EtoStepper();
				Stepper.Activated += HandleStepperActivated;
				Stepper.MinValue = double.NegativeInfinity;
				Stepper.MaxValue = double.PositiveInfinity;
				TextField.DoubleValue = Stepper.DoubleValue = 0;

				AddSubview(TextField);
				AddSubview(Stepper);
			}
		}

		public class EtoStepper : NSStepper
		{
			public override bool AcceptsFirstResponder()
			{
				return false;
			}
		}

		public override object EventObject
		{
			get { return Control.TextField; }
		}

		public static NSNumberFormatter DefaultFormatter = new NSNumberFormatter
		{
			NumberStyle = NSNumberFormatterStyle.Decimal,
			Lenient = true,
			UsesGroupingSeparator = false,
			MinimumFractionDigits = 0,
			MaximumFractionDigits = 0
		};

		protected override EtoNumericStepperView CreateControl() => new EtoNumericStepperView(this);

		static void HandleStepperActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)sender).Superview) as NumericStepperHandler;
			if (handler != null)
			{
				// prevent spinner from accumulating an inprecise value, which would eventually 
				// show values like 1.0000000000001 or 1.999999999998
				handler.Stepper.DoubleValue = double.Parse(handler.Stepper.DoubleValue.ToString());
				var val = handler.Stepper.DoubleValue;
				if (Math.Abs(val) < 1E-10)
				{
					handler.TextField.IntValue = 0;
				}
				else
				{
					handler.TextField.DoubleValue = handler.Stepper.DoubleValue;
				}
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		static void HandleTextChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)((NSNotification)sender).Object).Superview) as NumericStepperHandler;
			if (handler != null)
			{
				var formatter = (NSNumberFormatter)handler.TextField.Formatter;
				var str = handler.GetStringValue();
				var number = formatter.NumberFromString(str);
				if (number != null && number.DoubleValue >= handler.MinValue && number.DoubleValue <= handler.MaxValue)
				{
					handler.Stepper.DoubleValue = number.DoubleValue;
					handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
				}
			}
		}

		string GetStringValue()
		{
			var currentEditor = TextField.CurrentEditor;
			if (currentEditor != null)
				return currentEditor.Value;
			return TextField.StringValue;
		}

		protected override void Initialize()
		{
			base.Initialize();
			var size = GetNaturalSize(SizeF.PositiveInfinity);
			Control.Frame = new CGRect(0, 0, size.Width, size.Height);
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
			Widget.LostFocus += (sender, e) =>
			{
				var value = TextField.DoubleValue;
				var newValue = Math.Max(MinValue, Math.Min(MaxValue, TextField.DoubleValue));
				if (Math.Abs(value - newValue) > double.Epsilon || string.IsNullOrEmpty(TextField.StringValue))
				{
					TextField.DoubleValue = newValue;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
			};
			Widget.TextInput += (sender, e) =>
			{
				var formatter = (NSNumberFormatter)TextField.Formatter;
				if (NeedsFormat)
					return;
				if (e.Text == ".")
				{
					if (MaximumDecimalPlaces == 0 && DecimalPlaces == 0)
						e.Cancel = true;
					else
					{
						var str = GetStringValue();
						e.Cancel = str.Contains(formatter.DecimalSeparator);
						var editor = TextField.CurrentEditor;
						if (editor != null && editor.SelectedRange.Length > 0)
						{
							var sub = str.Substring((int)editor.SelectedRange.Location, (int)editor.SelectedRange.Length);
							e.Cancel &= !sub.Contains(formatter.DecimalSeparator);
						}
					}
				}
				else
				{
					foreach (var r in e.Text)
					{
						if (Char.IsDigit(r))
							continue;
						var str = r.ToString();
						if ((formatter.MaximumFractionDigits > 0 && str == formatter.DecimalSeparator)
							|| (formatter.UsesGroupingSeparator && str == formatter.GroupingSeparator)
							|| (MinValue < 0 && (str == formatter.NegativePrefix || str == formatter.NegativeSuffix))
							|| (MaxValue > 0 && (str == formatter.PositivePrefix || str == formatter.PositiveSuffix)))
							continue;
						e.Cancel = true;
						break;
					}
				}
			};
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Handled || ReadOnly)
				return;

			if (e.KeyData == Keys.Down)
			{
				var val = Value;
				var newval = Math.Max(val - Increment, MinValue);
				if (newval < val)
				{
					Value = newval;
				}
				e.Handled = true;
			}
			else if (e.KeyData == Keys.Up)
			{
				var val = Value;
				var newval = Math.Min(val + Increment, MaxValue);
				if (newval > val)
				{
					Value = newval;
				}
				e.Handled = true;
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (naturalSize == null)
			{
				var textSize = TextField.FittingSize;
				var stepperSize = Stepper.FittingSize;
				stepperSize = Stepper.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, stepperSize)).Size;
				var naturalHeight = Math.Max(textSize.Height, stepperSize.Height);
				naturalSize = new Size(80, (int)naturalHeight);
			}
			return naturalSize.Value;
		}

		public bool ReadOnly
		{
			get { return !TextField.Editable; }
			set
			{
				TextField.Editable = !value;
				Stepper.Enabled = TextField.Editable && TextField.Enabled;
			}
		}

		public double Value
		{
			get
			{
				var str = GetStringValue();
				var nsval = ((NSNumberFormatter)TextField.Formatter).NumberFromString(str);
				if (nsval == null)
					return 0;
				var value = nsval != null ? Math.Max(MinValue, Math.Min(MaxValue, nsval.DoubleValue)) : 0;
				if (!string.IsNullOrEmpty(FormatString))
					return value;
				value = Math.Round(value, MaximumDecimalPlaces);
				return value;
			}
			set
			{
				SetValue(value, Value);
			}
		}

		void SetValue(double value, double oldValue)
		{
			var val = Math.Max(MinValue, Math.Min(MaxValue, value));
			if (Math.Abs(oldValue - val) > double.Epsilon)
			{
				if (Math.Abs(val) < 1E-10)
				{
					Stepper.IntValue = TextField.IntValue = 0;
				}
				else
				{
					Stepper.DoubleValue = TextField.DoubleValue = val;
				}
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			}
		}

		public double MinValue
		{
			get { return Stepper.MinValue; }
			set
			{
				var oldValue = Value;
				Stepper.MinValue = value;
				SetValue(Value, oldValue);
			}
		}

		public double MaxValue
		{
			get { return Stepper.MaxValue; }
			set
			{
				var oldValue = Value;
				Stepper.MaxValue = value;
				SetValue(Value, oldValue);
			}
		}

		protected override bool ControlEnabled
		{
			get => TextField.Enabled;
			set
			{
				TextField.Enabled = value;
				Stepper.Enabled = TextField.Editable && TextField.Enabled;
			}
		}

		static readonly object Font_Key = new object();

		public Font Font
		{
			get { return Widget.Properties.Create(Font_Key, () => TextField.Font.ToEto()); }
			set
			{
				Widget.Properties.Set(Font_Key, value, () =>
				{
					TextField.Font = value.ToNS();
					TextField.SizeToFit();
					InvalidateMeasure();
				});
			}
		}

		public double Increment
		{
			get { return Stepper.Increment; }
			set { Stepper.Increment = value; }
		}

		static readonly object DecimalPlaces_Key = new object();

		public int DecimalPlaces
		{
			get { return Widget.Properties.Get<int>(DecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(DecimalPlaces_Key, value, () =>
				{
					MaximumDecimalPlaces = Math.Max(MaximumDecimalPlaces, DecimalPlaces);
					SetFormatter();
				});
			}
		}

		protected class EtoNumberFormatter : NSNumberFormatter
		{
			WeakReference handler;
			public NumericStepperHandler Handler
			{
				get => handler?.Target as NumericStepperHandler;
				set => handler = new WeakReference(value);
			}

			static IntPtr sel_getObjectValue = Selector.GetHandle("getObjectValue:forString:errorDescription:");

			string TrimNumericString(string text) => Regex.Replace(text, $"[ ]|({Regex.Escape(Handler.CultureInfo.NumberFormat.NumberGroupSeparator)})", "");

			bool NumberStringsMatch(string num1, string num2) => string.Compare(TrimNumericString(num1), TrimNumericString(num2), Handler.CultureInfo, CompareOptions.IgnoreCase) == 0;

			[Export("getObjectValue:forString:errorDescription:")]
			public bool GetObjectValue(IntPtr obj, IntPtr strPtr, IntPtr errorDescription)
			{
				// monomac can't handle out params that pass a null pointer (errorDescription), so we marshal manually here
				var h = Handler;
				if (h != null && h.NeedsFormat)
				{
					double result;
					var str = NSString.FromHandle(strPtr);
					var text = str;
					if (h.HasFormatString)
						text = Regex.Replace(text, $@"(?!\d|{Regex.Escape(h.CultureInfo.NumberFormat.NumberDecimalSeparator)}|{Regex.Escape(h.CultureInfo.NumberFormat.NegativeSign)}).", ""); // strip any non-numeric value
					if (double.TryParse(text, NumberStyles.Any, h.CultureInfo, out result))
					{
						// test to see if it matches the negative string format
						if (h.HasFormatString && result > 0 && NumberStringsMatch((-result).ToString(h.ComputedFormatString, h.CultureInfo), str))
							result = -result;

						var nsresult = new NSNumber(result);
						Marshal.WriteIntPtr(obj, 0, nsresult.Handle);
						return true;
					}
					// test to see if it matches the zero format which could be blank or some other text
					if (h.HasFormatString && NumberStringsMatch(0.0.ToString(h.ComputedFormatString, h.CultureInfo), str))
					{
						var nsresult = new NSNumber(0);
						Marshal.WriteIntPtr(obj, 0, nsresult.Handle);
						return true;
					}
				}
				return Messaging.bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr(SuperHandle, sel_getObjectValue, obj, strPtr, errorDescription);
			}

			public override string StringFor(NSObject value)
			{
				var h = Handler;
				var number = value as NSNumber;
				if (h != null && h.NeedsFormat && number != null)
				{
					var format = h.ComputedFormatString;
					return number.DoubleValue.ToString(format, h.CultureInfo);
				}
				return base.StringFor(value);
			}
		}

		void SetFormatter()
		{
			var formatter = new EtoNumberFormatter
			{
				Handler = this,
				NumberStyle = NSNumberFormatterStyle.Decimal,
				Lenient = true,
				UsesGroupingSeparator = false,
				MinimumFractionDigits = (nnint)DecimalPlaces,
				MaximumFractionDigits = (nnint)MaximumDecimalPlaces
			};

			Stepper.Formatter = formatter;
			TextField.Formatter = formatter;
			if (Widget.Loaded)
			{
				TextField.SetNeedsDisplay();
				var currentEditor = TextField.CurrentEditor;
				if (currentEditor != null)
				{
					currentEditor.Value = Stepper.StringValue ?? string.Empty;
				}
			}
		}

		public Color TextColor
		{
			get { return TextField.TextColor.ToEto(); }
			set { TextField.TextColor = value.ToNSUI(); }
		}

		protected override void SetBackgroundColor(Color? color)
		{
			if (color != null)
				TextField.BackgroundColor = color.Value.ToNSUI();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.TextInputEvent:
				case Eto.Forms.Control.LostFocusEvent:
					// Handled by MacFieldEditor
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static readonly object FormatString_Key = new object();

		public string FormatString
		{
			get { return Widget.Properties.Get<string>(FormatString_Key); }
			set
			{
				var old = FormatString;
				try
				{
					Widget.Properties.Set(FormatString_Key, value, SetFormatter);
				}
				catch
				{
					Widget.Properties.Set(FormatString_Key, old, SetFormatter);
					throw;
				}
				Widget.Properties.Remove(ComputedFormatString_Key);
			}
		}

		static readonly object ComputedFormatString_Key = new object();

		public string ComputedFormatString
		{
			get
			{
				var format = FormatString;
				if (!string.IsNullOrEmpty(format))
					return format;
				format = Widget.Properties.Get<string>(ComputedFormatString_Key);
				if (format == null)
				{
					format = "0.";
					if (DecimalPlaces > 0)
						format += new string('0', DecimalPlaces);
					if (MaximumDecimalPlaces > DecimalPlaces)
						format += new string('#', MaximumDecimalPlaces - DecimalPlaces);
					Widget.Properties.Set(ComputedFormatString_Key, format);
				}
				return format;
			}
		}

		bool NeedsFormat => HasFormatString || CultureInfo != CultureInfo.CurrentCulture;

		bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		static readonly object CultureInfo_Key = new object();

		public CultureInfo CultureInfo
		{
			get { return Widget.Properties.Get<CultureInfo>(CultureInfo_Key, CultureInfo.CurrentCulture); }
			set
			{
				Widget.Properties.Remove(ComputedFormatString_Key);
				Widget.Properties.Set(CultureInfo_Key, value, SetFormatter, CultureInfo.CurrentCulture);
			}
		}

		static readonly object MaximumDecimalDigits_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaximumDecimalDigits_Key); }
			set
			{
				Widget.Properties.Set(MaximumDecimalDigits_Key, value, () =>
				{
					DecimalPlaces = Math.Min(DecimalPlaces, MaximumDecimalPlaces);
					SetFormatter();
				});
			}
		}
	}
}