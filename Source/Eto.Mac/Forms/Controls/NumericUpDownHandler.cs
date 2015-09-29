using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Linq;


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
		Size? naturalSize;

		public override NSView ContainerControl { get { return Control; } }

		public override NSView FocusControl { get { return text; } }

		public NSTextField TextField { get { return text; } }

		public NSStepper Stepper { get { return stepper; } }

		public class EtoTextField : NSTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }
		}

		class EtoNumericUpDownView : NSView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

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

		public static NSNumberFormatter DefaultFormatter = new NSNumberFormatter
		{ 
			NumberStyle = NSNumberFormatterStyle.Decimal, 
			Lenient = true,
			UsesGroupingSeparator = false,
			MinimumFractionDigits = 0,
			MaximumFractionDigits = 0
		};

		public NumericUpDownHandler()
		{
			this.Control = new EtoNumericUpDownView
			{
				WeakHandler = new WeakReference(this),
				AutoresizesSubviews = false
			};
			text = new EtoTextField
			{
				WeakHandler = new WeakReference(this),
				Bezeled = true,
				Editable = true,
				Formatter = DefaultFormatter
			};
			text.Changed += HandleTextChanged;

			stepper = new EtoStepper();
			stepper.Activated += HandleStepperActivated;
			stepper.MinValue = double.NegativeInfinity;
			stepper.MaxValue = double.PositiveInfinity;
			text.DoubleValue = stepper.DoubleValue = 0;
			Control.AddSubview(text);
			Control.AddSubview(stepper);
		}

		static void HandleStepperActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)sender).Superview) as NumericUpDownHandler;
			if (handler != null)
			{
				// prevent spinner from accumulating an inprecise value, which would eventually 
				// show values like 1.0000000000001 or 1.999999999998
				handler.stepper.DoubleValue = double.Parse(handler.stepper.DoubleValue.ToString());
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
				var formatter = (NSNumberFormatter)handler.text.Formatter;
				var str = handler.GetStringValue();
				var number = formatter.NumberFromString(str);
				if (number != null && number.DoubleValue >= handler.MinValue && number.DoubleValue <= handler.MaxValue)
				{
					handler.stepper.DoubleValue = number.DoubleValue;
					handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
				}
			}
		}

		string GetStringValue()
		{
			var currentEditor = text.CurrentEditor;
			if (currentEditor != null)
				return currentEditor.Value;
			return text.StringValue;
		}

		protected override void Initialize()
		{
			base.Initialize();
			var size = GetNaturalSize(Size.MaxValue);
			Control.Frame = new CGRect(0, 0, size.Width, size.Height);
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
			Widget.LostFocus += (sender, e) =>
			{
				var value = text.DoubleValue;
				var newValue = Math.Max(MinValue, Math.Min(MaxValue, text.DoubleValue));
				if (Math.Abs(value - newValue) > double.Epsilon || string.IsNullOrEmpty(text.StringValue))
				{
					text.DoubleValue = newValue;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
			};
			Widget.TextInput += (sender, e) =>
			{
				var formatter = (NSNumberFormatter)text.Formatter;
				if (e.Text == ".")
				{
					if (MaximumDecimalPlaces == 0 && DecimalPlaces == 0)
						e.Cancel = true;
					else
					{
						var str = GetStringValue();
						e.Cancel = str.Contains(formatter.DecimalSeparator);
						var editor = text.CurrentEditor;
						if (editor != null && editor.SelectedRange.Length > 0)
						{
							var sub = str.Substring((int)editor.SelectedRange.Location, (int)editor.SelectedRange.Length);
							e.Cancel &= !sub.Contains(formatter.DecimalSeparator);
						}
					}
				}
				else
					e.Cancel = !e.Text.All(r =>
					{
						if (Char.IsDigit(r))
							return true;
						var str = r.ToString();
						return 
							(formatter.MaximumFractionDigits > 0 && str == formatter.DecimalSeparator)
						|| (formatter.UsesGroupingSeparator && str == formatter.GroupingSeparator)
						|| (MinValue < 0 && (str == formatter.NegativePrefix || str == formatter.NegativeSuffix))
						|| (MaxValue > 0 && (str == formatter.PositivePrefix || str == formatter.PositiveSuffix));
					});
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
				text.SizeToFit();
				stepper.SizeToFit();
				var naturalHeight = Math.Max(text.Frame.Height, stepper.Frame.Height);
				naturalSize = new Size(80, (int)naturalHeight);
			}
			return naturalSize.Value;
		}

		public bool ReadOnly
		{
			get { return !text.Editable; }
			set
			{ 
				text.Editable = !value;
				stepper.Enabled = text.Editable && text.Enabled;
			}
		}

		public double Value
		{
			get
			{ 
				var str = GetStringValue();
				var nsval = ((NSNumberFormatter)text.Formatter).NumberFromString(str);
				if (nsval == null)
					return 0;
				var value = nsval != null ? Math.Max(MinValue, Math.Min(MaxValue, nsval.DoubleValue)) : 0;
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
					stepper.IntValue = text.IntValue = 0;
				}
				else
				{
					stepper.DoubleValue = text.DoubleValue = val;
				}
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			}
		}

		public double MinValue
		{
			get { return stepper.MinValue; }
			set
			{
				var oldValue = Value;
				stepper.MinValue = value;
				SetValue(Value, oldValue);
			}
		}

		public double MaxValue
		{
			get { return stepper.MaxValue; }
			set
			{
				var oldValue = Value;
				stepper.MaxValue = value;
				SetValue(Value, oldValue);
			}
		}

		public override bool Enabled
		{
			get { return text.Enabled; }
			set
			{
				text.Enabled = value;
				stepper.Enabled = text.Editable && text.Enabled;
			}
		}

		static readonly object Font_Key = new object();

		public Font Font
		{
			get { return Widget.Properties.Create(Font_Key, () => text.Font.ToEto()); }
			set
			{
				Widget.Properties.Set(Font_Key, value, () =>
				{
					text.Font = value.ToNS();
					text.SizeToFit();
					LayoutIfNeeded();
				});
			}
		}

		public double Increment
		{
			get { return stepper.Increment; }
			set { stepper.Increment = value; }
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

		void SetFormatter()
		{
			var formatter = new NSNumberFormatter
			{
				NumberStyle = NSNumberFormatterStyle.Decimal, 
				Lenient = true,
				UsesGroupingSeparator = false,
				MinimumFractionDigits = (nnint)DecimalPlaces,
				MaximumFractionDigits = (nnint)MaximumDecimalPlaces
			};

			text.Formatter = formatter;
			if (Widget.Loaded)
				text.SetNeedsDisplay();
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

		static readonly object CustomFieldEditorKey = new object();

		public override NSObject CustomFieldEditor { get { return Widget.Properties.Get<NSObject>(CustomFieldEditorKey); } }

		public void SetCustomFieldEditor()
		{
			if (CustomFieldEditor != null)
				return;
			Widget.Properties[CustomFieldEditorKey] = new CustomTextFieldEditor
			{
				WeakHandler = new WeakReference(this)
			};
		}

		static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");
		static readonly IntPtr selInsertText = Selector.GetHandle("insertText:");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.TextInputEvent:
					SetCustomFieldEditor();
					AddMethod(selInsertText, new Action<IntPtr, IntPtr, IntPtr>(TriggerTextInput), "v@:@", CustomFieldEditor);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					SetCustomFieldEditor();
					// lost focus is on the custom field editor, not on the control itself (it loses focus immediately due to the field editor)
					AddMethod(selResignFirstResponder, new Func<IntPtr, IntPtr, bool>(TriggerLostFocus), "B@:", CustomFieldEditor);
					break;
				default:
					base.AttachEvent(id);
					break;
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