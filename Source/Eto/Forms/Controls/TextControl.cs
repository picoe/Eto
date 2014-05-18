using System;

namespace Eto.Forms
{
	[ContentProperty("Text")]
	public abstract class TextControl : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static TextControl()
		{
			EventLookup.Register<TextControl>(c => c.OnTextChanged(null), TextControl.TextChangedEvent);
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TextControl(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		protected TextControl()
		{
		}

		public const string TextChangedEvent = "TextControl.TextChanged";

		public event EventHandler<EventArgs> TextChanged
		{
			add { Properties.AddHandlerEvent(TextChangedEvent, value); }
			remove { Properties.RemoveEvent(TextChangedEvent, value); }
		}

		protected virtual void OnTextChanged(EventArgs e)
		{
			Properties.TriggerEvent(TextChangedEvent, this, e);
		}

		public virtual string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

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

		public new interface ICallback : CommonControl.ICallback
		{
			void OnTextChanged(TextControl widget, EventArgs e);
		}

		protected new class Callback : CommonControl.Callback, ICallback
		{
			public void OnTextChanged(TextControl widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnTextChanged(e));
			}
		}

		public new interface IHandler : CommonControl.IHandler
		{
			string Text { get; set; }
		}
	}
}

