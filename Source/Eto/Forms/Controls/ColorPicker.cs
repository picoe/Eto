using System;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control to show and pick a color.
	/// </summary>
	[Handler(typeof(ColorPicker.IHandler))]
	public class ColorPicker : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="ValueChanged"/> event.
		/// </summary>
		public const string ColorChangedEvent = "ColorPicker.ValueChanged";

		/// <summary>
		/// Occurs when the <see cref="Value"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged
		{
			add { Properties.AddHandlerEvent(ColorChangedEvent, value); }
			remove { Properties.RemoveEvent(ColorChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see name="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnColorChanged(EventArgs e)
		{
			Properties.TriggerEvent(ColorChangedEvent, this, e);
		}

		/// <summary>
		/// Gets or sets the selected color value.
		/// </summary>
		/// <value>The currently selected color value.</value>
		public Color Value
		{
			get { return Handler.Color; }
			set { Handler.Color = value; }
		}

		/// <summary>
		/// Gets a binding to the <see cref="Value"/> property.
		/// </summary>
		/// <value>The value binding.</value>
		public ControlBinding<ColorPicker, Color> ValueBinding
		{
			get
			{
				return new ControlBinding<ColorPicker,Color>(
					this,
					r => r.Value,
					(r,val) => r.Value = val,
					(r, ev) => r.ValueChanged += ev,
					(r, ev) => r.ValueChanged -= ev
				);
			}
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for <see cref="ColorPicker"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the color changed event.
			/// </summary>
			void OnColorChanged(ColorPicker widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="ColorPicker"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the color changed event.
			/// </summary>
			public void OnColorChanged(ColorPicker widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnColorChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="ColorPicker"/> control
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the selected color.
			/// </summary>
			/// <value>The selected color.</value>
			Color Color { get; set; }
		}
	}
}
