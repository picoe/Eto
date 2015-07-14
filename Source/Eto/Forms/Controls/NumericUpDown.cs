using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Control for the user to enter a numeric value
	/// </summary>
	/// <remarks>
	/// This usually presents with a spinner to increase/decrease the value, or a specific numeric keyboard.
	/// </remarks>
	[Handler(typeof(NumericUpDown.IHandler))]
	public class NumericUpDown : CommonControl
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
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.NumericUpDown"/> is read only.
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
		/// <value>The number of decimal places.</value>
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
		/// Gets the binding for the <see cref="Value"/> property.
		/// </summary>
		/// <value>The value binding.</value>
		public BindableBinding<NumericUpDown, double> ValueBinding
		{
			get
			{
				return new BindableBinding<NumericUpDown, double>(
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
		/// Callback interface for the <see cref="NumericUpDown"/>
		/// </summary>
		public new interface ICallback : CommonControl.ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			void OnValueChanged(NumericUpDown widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="NumericUpDown"/>
		/// </summary>
		protected new class Callback : CommonControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			public void OnValueChanged(NumericUpDown widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnValueChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="NumericUpDown"/> control.
		/// </summary>
		public new interface IHandler : CommonControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.NumericUpDown"/> is read only.
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
			/// <value>The number of decimal places.</value>
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
		}
	}
}
