using System;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for controls implementing text and text changing events, when it has a singular text property.
	/// </summary>
	/// <remarks>
	/// If a control has multiple text properties, it is best to implement those specifically to identify what the property
	/// represents more clearly.
	/// </remarks>
	[ContentProperty("Text")]
	public abstract class TextControl : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static TextControl()
		{
			EventLookup.Register<TextControl>(c => c.OnTextChanged(null), TextControl.TextChangedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextControl"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TextControl(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextControl"/> class.
		/// </summary>
		protected TextControl()
		{
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="TextChanged"/> event
		/// </summary>
		public const string TextChangedEvent = "TextControl.TextChanged";

		/// <summary>
		/// Occurs when the <see cref="Text"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> TextChanged
		{
			add { Properties.AddHandlerEvent(TextChangedEvent, value); }
			remove { Properties.RemoveEvent(TextChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnTextChanged(EventArgs e)
		{
			Properties.TriggerEvent(TextChangedEvent, this, e);
		}

		/// <summary>
		/// Gets or sets the text of the control.
		/// </summary>
		/// <value>The text content.</value>
		public virtual string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets the binding for the <see cref="Text"/> property.
		/// </summary>
		/// <value>The text binding.</value>
		public ObjectBinding<TextControl, string> TextBinding
		{
			get
			{
				return new ObjectBinding<TextControl, string>(
					this,
					c => c.Text,
					(c, v) => c.Text = v,
					(c, h) => c.TextChanged += h,
					(c, h) => c.TextChanged -= h
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
		/// Callback interface for the <see cref="TextControl"/> based controls
		/// </summary>
		public new interface ICallback : CommonControl.ICallback
		{
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			void OnTextChanged(TextControl widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="TextControl"/> based controls
		/// </summary>
		protected new class Callback : CommonControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			public void OnTextChanged(TextControl widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnTextChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="TextControl"/> based controls
		/// </summary>
		public new interface IHandler : CommonControl.IHandler
		{
			/// <summary>
			/// Gets or sets the text of the control.
			/// </summary>
			/// <value>The text content.</value>
			string Text { get; set; }
		}
	}
}

