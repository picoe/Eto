using Eto.Mac.Drawing;
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
			public EtoTextField(IntPtr handle) : base(handle)
			{
			}
			public EtoTextField()
			{
				Cell = new EtoTextFieldCell();
			}
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
				Stepper.MinValue = double.MinValue;
				Stepper.MaxValue = double.MaxValue;
				Stepper.ValueWraps = false;
				TextField.DoubleValue = Stepper.DoubleValue = 0;

				AddSubview(TextField);
				AddSubview(Stepper);

				this.SetClipsToBounds(false);
			}
		}

		public class EtoStepper : NSStepper
		{
			public override bool AcceptsFirstResponder()
			{
				return false;
			}
		}

		public override object EventObject => Control.TextField;

		protected override IColorizeCell ColorizeCell => Control.TextField.Cell as IColorizeCell;

		public static NSNumberFormatter DefaultFormatter = new NSNumberFormatter
		{
			NumberStyle = NSNumberFormatterStyle.Decimal,
			Lenient = true,
			UsesGroupingSeparator = false,
			MinimumFractionDigits = 0,
			MaximumFractionDigits = 0
		};

		protected override EtoNumericStepperView CreateControl() => new EtoNumericStepperView(this);

		static double GetPreciseValue(double value)
		{
			// prevent spinner from accumulating an inprecise value, which would eventually 
			// show values like 1.0000000000001 or 1.999999999998

			// note: some versions of mono can crash roundtripping via ToString() with MaxValue, so use TryParse
			var str = value.ToString("G15");
			if (double.TryParse(str, out var val))
				return val;
			else
				return value;
		}

		static void HandleStepperActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)sender).Superview) as NumericStepperHandler;
			if (handler != null)
			{
				var val = GetPreciseValue(handler.Stepper.DoubleValue);

				if (Math.Abs(val) < 1E-10)
				{
					handler.TextField.IntValue = 0;
				}
				else
				{
					handler.TextField.DoubleValue = val;
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
			// EtoNumberFormatter is what applies CultureInfo when NeedsFormat; DefaultFormatter is a plain
			// NSNumberFormatter and can only ever format with the OS locale, so replace it up front
			SetFormatter();
			var size = GetNaturalSize(SizeF.PositiveInfinity);
			Control.Frame = new CGRect(0, 0, size.Width, size.Height);
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
			Widget.LostFocus += (sender, e) =>
			{
				InvalidateOSFormat();
				EnsureFormatter();
				var value = TextField.DoubleValue;
				var newValue = Math.Max(MinValue, Math.Min(MaxValue, value));
				if (Math.Abs(value - newValue) > double.Epsilon || string.IsNullOrEmpty(TextField.StringValue))
				{
					TextField.DoubleValue = newValue;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
			};
			Widget.TextInput += (sender, e) =>
			{
				// with a format string the text isn't necessarily a plain number, so don't filter what can be typed
				if (HasFormatString)
					return;

				InvalidateOSFormat();
				EnsureFormatter();

				// filter using the symbols of whichever formatter is in effect: CultureInfo's when we format the value
				// ourselves, and the native formatter's otherwise - those follow the OS locale, including whatever the
				// user has customized about it.  Using CultureInfo's for the native case rejects the separator the
				// field actually displays and parses with, so the value could not be typed at all.
				var needsFormat = NeedsFormat;
				var format = CultureInfo.NumberFormat;
				var nativeFormat = (NSNumberFormatter)TextField.Formatter;
				var decimalSeparator = needsFormat ? format.NumberDecimalSeparator : nativeFormat.DecimalSeparator;
				var negativeSign = needsFormat ? format.NegativeSign : nativeFormat.MinusSign;
				var positiveSign = needsFormat ? format.PositiveSign : nativeFormat.PlusSign;
				var allowDecimal = MaximumDecimalPlaces > 0;

				if (e.Text == decimalSeparator)
				{
					// only one decimal separator is allowed, unless the existing one is being replaced
					if (!allowDecimal)
						e.Cancel = true;
					else
					{
						var str = GetStringValue();
						e.Cancel = str.Contains(decimalSeparator);
						var editor = TextField.CurrentEditor;
						if (editor != null && editor.SelectedRange.Length > 0)
						{
							var sub = str.Substring((int)editor.SelectedRange.Location, (int)editor.SelectedRange.Length);
							e.Cancel &= !sub.Contains(decimalSeparator);
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
						if ((allowDecimal && str == decimalSeparator)
							|| (MinValue < 0 && str == negativeSign)
							|| (MaxValue > 0 && str == positiveSign))
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
				var newval = val - Increment;
				if (Wrap && newval < MinValue)
					Value = MaxValue;
				else
				{
					newval = Math.Max(GetPreciseValue(newval), MinValue);
					if (newval < val)
						Value = newval;
				}
				e.Handled = true;
			}
			else if (e.KeyData == Keys.Up)
			{
				var val = Value;
				var newval = val + Increment;
				if (Wrap && newval > MaxValue)
					Value = MinValue;
				else
				{
					newval = Math.Min(GetPreciseValue(newval), MaxValue);
					if (newval > val)
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
				EnsureFormatter();
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
#if USE_CFSTRING
					var str = CFString.FromHandle(strPtr);
#else
					var str = NSString.FromHandle(strPtr);
#endif
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
			// the computed format string depends on the decimal places, which may have changed
			Widget.Properties.Remove(ComputedFormatString_Key);

			// remember what the OS was formatting like, so EnsureFormatter can tell when it has changed
			_osFormat = OSFormat;

			var formatter = new EtoNumberFormatter
			{
				Handler = this,
				NumberStyle = NSNumberFormatterStyle.Decimal,
				Lenient = true,
				UsesGroupingSeparator = false,
				MinimumFractionDigits = DecimalPlaces,
				MaximumFractionDigits = MaximumDecimalPlaces
			};

			Stepper.Formatter = formatter;
			TextField.Formatter = formatter;
			if (Widget.Loaded)
			{
				TextField.NeedsDisplay = true;
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

		protected override bool UseColorizeCellWithAlphaOnly => true;

		protected override void SetBackgroundColor(Color? color)
		{
			base.SetBackgroundColor(color);
			var textField = Control.TextField;
			var c = color ?? Colors.Transparent;
			textField.BackgroundColor = c.ToNSUI();
			textField.DrawsBackground = c.A > 0;
			textField.WantsLayer = c.A < 1;
			if (Widget.Loaded && HasFocus)
			{
				var editor = textField.CurrentEditor;
				if (editor != null)
				{
					var nscolor = c.ToNSUI();
					editor.BackgroundColor = nscolor;
					editor.DrawsBackground = c.A > 0;
				}
			}
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

		bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		/// <summary>
		/// Gets whether the value has to be formatted and parsed using <see cref="CultureInfo"/> instead of being
		/// left to the native formatter.
		/// </summary>
		/// <remarks>
		/// When the culture is the one the OS is set to, the native formatter is left to do the work so that whatever
		/// the user has customized about their number format is respected - macOS lets the separators be changed
		/// independently of the region, and CultureInfo cannot represent that.  Any other culture was asked for
		/// explicitly and has to be applied, even when it happens to be what CurrentCulture is set to.
		/// </remarks>
		bool NeedsFormat => HasFormatString || !IsOSCulture;

		/// <summary>
		/// Gets whether <see cref="CultureInfo"/> is the culture the OS is set to.
		/// </summary>
		/// <remarks>
		/// Compared by name rather than by instance: <see cref="CultureInfo.CurrentCulture"/> is not necessarily the
		/// OS culture, since an app can set it to anything - and an explicitly assigned culture is never the same
		/// instance as CurrentCulture even when it names the same culture.
		/// </remarks>
		bool IsOSCulture => string.Equals(CultureInfo.Name, OSFormat.Culture, StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// Gets the name of the OS locale's culture in the form <see cref="CultureInfo.Name"/> uses, e.g. the
		/// identifier en_CA becomes en-CA.
		/// </summary>
		/// <remarks>
		/// Read from the autoupdating locale so it follows the user changing their region.  Locale identifiers can
		/// carry keywords (en_CA@currency=CAD) which have no CultureInfo equivalent, so those are dropped.
		/// </remarks>
		static string OSCultureName
		{
			get
			{
				var identifier = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier ?? string.Empty;
				var keyword = identifier.IndexOf('@');
				if (keyword >= 0)
					identifier = identifier.Substring(0, keyword);
				return identifier.Replace('_', '-');
			}
		}

		(string Culture, string Decimal, string Group, string Minus) _osFormat;

		static (string Culture, string Decimal, string Group, string Minus)? s_osFormat;
		static NSObject[] s_osObservers;

		/// <summary>
		/// Gets what the OS formats numbers like, to detect the user changing it.
		/// </summary>
		/// <remarks>
		/// NSNumberFormatter reads its symbols from the locale when it is created and keeps them, so one built before
		/// the user changed their number format still uses the old separators.  The text and the formatter that parses
		/// it would then disagree, reading a "123,123" typed earlier as 123123.  The culture name is part of it as it
		/// decides <see cref="IsOSCulture"/>, and so whether the native formatter is used at all.
		///
		/// Cached: reading it builds an NSNumberFormatter, which is far too slow to repeat on every value read.  It is
		/// dropped when the locale changes or the app becomes active, and by <see cref="InvalidateOSFormat"/> whenever
		/// the user interacts with a stepper.
		/// </remarks>
		static (string Culture, string Decimal, string Group, string Minus) OSFormat
		{
			get
			{
				if (s_osFormat == null)
				{
					// changing the number format means leaving for System Settings and coming back, so becoming
					// active is a better signal than the locale notification, which does not report a format
					// customized within the same locale
					s_osObservers ??= new[]
					{
						NSNotificationCenter.DefaultCenter.AddObserver(NSLocale.CurrentLocaleDidChangeNotification, n => s_osFormat = null),
						NSNotificationCenter.DefaultCenter.AddObserver(NSApplication.DidBecomeActiveNotification, n => s_osFormat = null)
					};
					var formatter = new NSNumberFormatter { NumberStyle = NSNumberFormatterStyle.Decimal };
					s_osFormat = (OSCultureName, formatter.DecimalSeparator, formatter.GroupingSeparator, formatter.MinusSign);
				}
				return s_osFormat.Value;
			}
		}

		/// <summary>
		/// Discards the cached OS format, so the next read picks up a change the locale notification did not report.
		/// </summary>
		static void InvalidateOSFormat() => s_osFormat = null;

		/// <summary>
		/// Rebuilds the formatter when the OS number format has changed since it was created, which re-renders the
		/// text from the value rather than leaving it to be reinterpreted with different separators.
		/// </summary>
		/// <remarks>
		/// Called from the points that read the text or the formatter's symbols.  It is not called while formatting,
		/// as that runs inside the formatter itself.
		/// </remarks>
		void EnsureFormatter()
		{
			if (_osFormat != OSFormat)
				SetFormatter();
		}

		static readonly object CultureInfo_Key = new object();

		public CultureInfo CultureInfo
		{
			get { return Widget.Properties.Get<CultureInfo>(CultureInfo_Key, CultureInfo.CurrentCulture); }
			set
			{
				Widget.Properties.Set(CultureInfo_Key, value, CultureInfo.CurrentCulture);
				// Set() removes the key when the value matches CultureInfo.CurrentCulture, so it can't report whether
				// the effective culture changed - reformat unconditionally.
				SetFormatter();
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

		public bool Wrap
		{
			get => Stepper.ValueWraps;
			set => Stepper.ValueWraps = value;
		}
		
		public TextAlignment TextAlignment
		{
			get => TextField.Alignment.ToEto();
			set => TextField.Alignment = value.ToNS();
		}
	}
}