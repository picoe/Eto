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

		public override NSView ContainerControl { get { return Control; } }

		public override NSView FocusControl { get { return Control.TextField; } }

		public NSTextField TextField { get { return Control.TextField; } }

		public NSStepper Stepper { get { return Control.Stepper; } }

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

		protected override EtoNumericStepperView CreateControl()
		{
			return new EtoNumericStepperView(this);
		}

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
			var size = GetNaturalSize(Size.MaxValue);
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
				TextField.SizeToFit();
				Stepper.SizeToFit();
				var naturalHeight = Math.Max(TextField.Frame.Height, Stepper.Frame.Height);
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

		public override bool Enabled
		{
			get { return TextField.Enabled; }
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
					LayoutIfNeeded();
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

			TextField.Formatter = formatter;
			if (Widget.Loaded)
			{
				TextField.SetNeedsDisplay();
				var currentEditor = TextField.CurrentEditor;
				if (currentEditor != null)
				{
					currentEditor.Value = TextField.StringValue;
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