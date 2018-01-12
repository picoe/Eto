using System;
using System.ComponentModel;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Control for the user to enter a numeric value (obsolete, use NumericStepper instead)
	/// </summary>
	/// <remarks>
	/// This usually presents with a <see cref="Stepper"/> to increase/decrease the value, or a specific numeric keyboard.
	/// </remarks>
	[Obsolete("Since 2.4: Use NumericStepper instead")]
	public class NumericUpDown : NumericStepper
	{
	}

	/// <summary>
	/// Control for the user to enter a numeric value
	/// </summary>
	/// <remarks>
	/// This usually presents with a <see cref="Stepper"/> to increase/decrease the value, or a specific numeric keyboard.
	/// </remarks>
	[Handler(typeof(NumericStepper.IHandler))]
	public class NumericStepper : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when the <see cref="Value"/> changed.
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.NumericStepper"/> is read only.
		/// </summary>
		/// <remarks>
		/// A read only control can copy the value and focus the control, but cannot edit or change the value.
		/// </remarks>
		/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <remarks>
		/// The value will be limited to a value between the <see cref="MinValue"/> and <see cref="MaxValue"/>.
		/// </remarks>
		/// <value>The value.</value>
		public double Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		/// <summary>
		/// Gets or sets the minimum value that can be entered.
		/// </summary>
		/// <remarks>
		/// Changing this will limit the current <see cref="Value"/> of the control.
		/// </remarks>
		/// <value>The minimum value.</value>
		[DefaultValue(double.MinValue)]
		public double MinValue
		{
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum value that can be entered.
		/// </summary>
		/// <remarks>
		/// Changing this will limit the current <see cref="Value"/> of the control.
		/// </remarks>
		/// <value>The maximum value.</value>
		[DefaultValue(double.MaxValue)]
		public double MaxValue
		{
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <remarks>
		/// By default, the text will get a color based on the user's theme. However, this is usually black.
		/// </remarks>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		/// <summary>
		/// Gets or sets the number of digits to display after the decimal.
		/// </summary>
		/// <remarks>
		/// The NumericStepper control will at least show the number of fraction digits as specified by this value, padded
		/// by zeros. 
		/// The <see cref="MaximumDecimalPlaces"/> specifies the maximum number of fraction digits the control will display
		/// if the value has a value that can be represented by more digits.
		/// The <see cref="Value"/> property is rounded to the number of fraction digits specified by <see cref="MaximumDecimalPlaces"/>.
		/// 
		/// Note that this does not apply if you have specified <see cref="FormatString"/>
		/// </remarks>
		/// <example>
		/// This shows the effect of the <see cref="DecimalPlaces"/> and <see cref="MaximumDecimalPlaces"/> on the display 
		/// of the control and its returned value.
		/// <pre>
		/// var numeric = new NumericStepper();
		/// 
		/// numeric.DecimalPlaces = 2;
		/// numeric.MaximumDecimalPlaces = 4;
		/// 
		/// numeric.Value = 123;         // control will display "123.00"
		/// numeric.Value = 123.45;      // control will display "123.45"
		/// numeric.Value = 123.4567;    // control will display "123.4567"
		/// numeric.Value = 123.4567890; // control will display "123.4568"
		/// </pre>
		/// </example>
		/// <value>The number of decimal places to always show.</value>
		/// <seealso cref="MaximumDecimalPlaces"/>
		/// <seealso cref="FormatString"/>
		public int DecimalPlaces
		{
			get { return Handler.DecimalPlaces; }
			set { Handler.DecimalPlaces = value; }
		}

		/// <summary>
		/// Gets or sets the value to increment when the user clicks on the stepper buttons.
		/// </summary>
		/// <value>The step increment.</value>
		public double Increment
		{
			get { return Handler.Increment; }
			set { Handler.Increment = value; }
		}

		/// <summary>
		/// Gets or sets the maximum number of decimal places that can be shown.
		/// </summary>
		/// <remarks>
		/// Specifies how many fraction digits can be shown if required to display the specified <see cref="Value"/>.
		/// The number of digits shown will be at least the number of digits specified by <see cref="DecimalPlaces"/>.
		/// The <see cref="Value"/> and the display is rounded to the number of fraction digits specified by this value.
		/// <see cref="DecimalPlaces"/> for an example of how the MaximumDecimalPlaces can be used.
		/// 
		/// Note that this does not apply if you have specified <see cref="FormatString"/>.
		/// </remarks>
		/// <value>The maximum number of decimal places that will be shown.</value>
		/// <seealso cref="MaximumDecimalPlaces"/>
		/// <seealso cref="FormatString"/>
		public int MaximumDecimalPlaces
		{
			get { return Handler.MaximumDecimalPlaces; }
			set { Handler.MaximumDecimalPlaces = value; }
		}

		/// <summary>
		/// Gets or sets the format string for the display of the numeric value.
		/// </summary>
		/// <remarks>
		/// This can be used to specify standard or custom format strings used via <see cref="Double.ToString(string, IFormatProvider)"/>.
		/// The exact output is determined using the specified <see cref="CultureInfo"/>.
		/// 
		/// For example "c" would show a currency value.
		/// 
		/// Any extra non-numeric or separator characters are stripped when parsing the string so that you can include extra (non-numeric) 
		/// string values while still allowing the user to change the numeric string.
		/// </remarks>
		public string FormatString
		{
			get { return Handler.FormatString; }
			set { Handler.FormatString = value; }
		}

		/// <summary>
		/// Specifies the culture to show the numeric value in (default is <see cref="CultureInfo.CurrentCulture"/>).
		/// </summary>
		/// <remarks>
		/// This is used to format the numeric value, and when using the <see cref="FormatString"/> it determines the character(s) used
		/// for the thousands separator, decimal separator, and currency symbol.
		/// </remarks>
		public CultureInfo CultureInfo
		{
			get { return Handler.CultureInfo; }
			set { Handler.CultureInfo = value; }
		}

		/// <summary>
		/// Gets the binding for the <see cref="Value"/> property.
		/// </summary>
		/// <value>The value binding.</value>
		public BindableBinding<NumericStepper, double> ValueBinding
		{
			get
			{
				return new BindableBinding<NumericStepper, double>(
					this, 
					c => c.Value, 
					(c, v) => c.Value = v, 
					(c, h) => c.ValueChanged += h, 
					(c, h) => c.ValueChanged -= h
				)
				{
					SettingNullValue = 0
				};
			}
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for the <see cref="NumericStepper"/>
		/// </summary>
		public new interface ICallback : CommonControl.ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			void OnValueChanged(NumericStepper widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="NumericStepper"/>
		/// </summary>
		protected new class Callback : CommonControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			public void OnValueChanged(NumericStepper widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnValueChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="NumericStepper"/> control.
		/// </summary>
		public new interface IHandler : CommonControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.NumericStepper"/> is read only.
			/// </summary>
			/// <remarks>
			/// A read only control can copy the value and focus the control, but cannot edit or change the value.
			/// </remarks>
			/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
			bool ReadOnly { get; set; }

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			/// <remarks>
			/// The value will be limited to a value between the <see cref="MinValue"/> and <see cref="MaxValue"/>.
			/// </remarks>
			/// <value>The value.</value>
			double Value { get; set; }

			/// <summary>
			/// Gets or sets the minimum value that can be entered.
			/// </summary>
			/// <remarks>
			/// Changing this will limit the current <see cref="Value"/> of the control.
			/// </remarks>
			/// <value>The minimum value.</value>
			double MinValue { get; set; }

			/// <summary>
			/// Gets or sets the maximum value that can be entered.
			/// </summary>
			/// <remarks>
			/// Changing this will limit the current <see cref="Value"/> of the control.
			/// </remarks>
			/// <value>The maximum value.</value>
			double MaxValue { get; set; }

			/// <summary>
			/// Gets or sets the number of digits to display after the decimal.
			/// </summary>
			/// <remarks>
			/// The NumericStepper control will at least show the number of fraction digits as specified by this value, padded
			/// by zeros. 
			/// The <see cref="MaximumDecimalPlaces"/> specifies the maximum number of fraction digits the control will display
			/// if the value has a value that can be represented by more digits.
			/// The <see cref="Value"/> property is rounded to the number of fraction digits specified by <see cref="MaximumDecimalPlaces"/>.
			/// </remarks>
			int DecimalPlaces { get; set; }

			/// <summary>
			/// Gets or sets the value to increment when the user clicks on the stepper buttons.
			/// </summary>
			/// <value>The step increment.</value>
			double Increment { get; set; }

			/// <summary>
			/// Gets or sets the color of the text.
			/// </summary>
			/// <remarks>
			/// By default, the text will get a color based on the user's theme. However, this is usually black.
			/// </remarks>
			/// <value>The color of the text.</value>
			Color TextColor { get; set; }

			/// <summary>
			/// Gets or sets the maximum number of decimal places that can be shown.
			/// </summary>
			/// <remarks>
			/// Specifies how many fraction digits can be shown if required to display the specified <see cref="Value"/>.
			/// The number of digits shown will be at least the number of digits specified by <see cref="DecimalPlaces"/>.
			/// The <see cref="Value"/> and the display is rounded to the number of fraction digits specified by this value.
			/// </remarks>
			/// <value>The maximum number of decimal places that will be shown.</value>
			/// <seealso cref="MaximumDecimalPlaces"/>
			int MaximumDecimalPlaces { get; set; }

			/// <summary>
			/// Gets or sets the format string for the display of the numeric value.
			/// </summary>
			/// <remarks>
			/// This can be used to specify standard or custom format strings used via <see cref="Double.ToString(string, IFormatProvider)"/>.
			/// The exact output is determined using the specified <see cref="CultureInfo"/>.
			/// 
			/// For example "c" would show a currency value.
			/// 
			/// Any extra non-numeric or separator characters are stripped when parsing the string so that you can include extra (non-numeric) 
			/// string values while still allowing the user to change the numeric string.
			/// </remarks>
			string FormatString { get; set; }

			/// <summary>
			/// Specifies the culture to show the numeric value in (default is <see cref="CultureInfo.CurrentCulture"/>).
			/// </summary>
			/// <remarks>
			/// This is used to format the numeric value, and when using the <see cref="FormatString"/> it determines the character(s) used
			/// for the thousands separator, decimal separator, and currency symbol.
			/// </remarks>
			CultureInfo CultureInfo { get; set; }
		}
	}
}
