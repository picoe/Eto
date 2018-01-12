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
		/// Gets or sets a value indicating whether the user can adjust the Alpha component of the Color.
		/// </summary>
		/// <remarks>
		/// This may or may not be supported in all platforms (e.g. WinForms).  
		/// Use <see cref="SupportsAllowAlpha"/> to determine if the current platform supports this feature.
		/// </remarks>
		/// <value><c>true</c> to allow adjustment of alpha; otherwise, <c>false</c>.</value>
		/// <seealso cref="SupportsAllowAlpha"/>
		public bool AllowAlpha
		{
			get { return Handler.AllowAlpha; }
			set { Handler.AllowAlpha = value; }
		}

		/// <summary>
		/// Gets a value indicating that the current platform supports the <see cref="AllowAlpha"/> property.
		/// </summary>
		/// <remarks>
		/// If not supported, the setting will be ignored.
		/// </remarks>
		/// <value><c>true</c> AllowAlpha is supported; otherwise, <c>false</c>.</value>
		/// <seealso cref="AllowAlpha"/>
		public bool SupportsAllowAlpha => Handler.SupportsAllowAlpha;

		/// <summary>
		/// Gets a binding to the <see cref="Value"/> property.
		/// </summary>
		/// <value>The value binding.</value>
		public BindableBinding<ColorPicker, Color> ValueBinding
		{
			get
			{
				return new BindableBinding<ColorPicker,Color>(
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
				using (widget.Platform.Context)
					widget.OnColorChanged(e);
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

			/// <summary>
			/// Gets or sets a value indicating whether the user can adjust the Alpha component of the Color.
			/// </summary>
			/// <remarks>
			/// This may or may not be supported in all platforms (e.g. WinForms).  
			/// Use <see cref="SupportsAllowAlpha"/> to determine if the current platform supports this feature.
			/// </remarks>
			/// <value><c>true</c> to allow adjustment of alpha; otherwise, <c>false</c>.</value>
			/// <seealso cref="SupportsAllowAlpha"/>
			bool AllowAlpha { get; set; }

			/// <summary>
			/// Gets a value indicating that the current platform supports the <see cref="AllowAlpha"/> property.
			/// </summary>
			/// <remarks>
			/// If not supported, the setting will be ignored.
			/// </remarks>
			/// <value><c>true</c> AllowAlpha is supported; otherwise, <c>false</c>.</value>
			/// <seealso cref="AllowAlpha"/>
			bool SupportsAllowAlpha { get; }
		}
	}
}
