using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Control to show and pick a font.
	/// </summary>
	[Handler(typeof(FontPicker.IHandler))]
	public class FontPicker : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations.
		/// </summary>
		/// <returns>The callback instance to use for this widget.</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.FontPicker"/> class.
		/// </summary>
		public FontPicker()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.FontPicker"/> class.
		/// </summary>
		/// <param name="font">Font to set as the current value..</param>
		public FontPicker(Font font)
		{
			Value = font;
		}

		/// <summary>
		/// Gets or sets the currently selected font.
		/// </summary>
		/// <value>The selected font.</value>
		public Font Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="FontPicker"/>.
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the currently selected font.
			/// </summary>
			/// <value>The selected font.</value>
			Font Value { get; set; }
		}

		// Font Changed Event

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="ValueChanged"/> event.
		/// </summary>
		public const string ValueChangedEvent = "FontPicker.ValueChanged";

		/// <summary>
		/// Occurs when the <see cref="Font"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged
		{
			add { Properties.AddHandlerEvent(ValueChangedEvent, value); }
			remove { Properties.RemoveEvent(ValueChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			Properties.TriggerEvent(ValueChangedEvent, this, e);
		}

		/// <summary>
		/// Gets a new binding for the <see cref="Value"/> property.
		/// </summary>
		/// <value>A new value binding.</value>
		public BindableBinding<FontPicker, Font> ValueBinding
		{
			get
			{
				return new BindableBinding<FontPicker, Font>(
					this,
					c => c.Value,
					(c, v) => c.Value = v,
					(c, h) => c.ValueChanged += h,
					(c, h) => c.ValueChanged -= h
				);
			}
		}

		/// <summary>
		/// Callback interface for handlers of the <see cref="FontPicker"/>.
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			void OnValueChanged(FontPicker widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="FontPicker"/>.
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			public void OnValueChanged(FontPicker widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnValueChanged(e);
			}
		}
	}
}
