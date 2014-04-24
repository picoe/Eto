using System;

namespace Eto.Forms
{
	public interface ITextControl : ICommonControl
	{
		string Text { get; set; }
	}

	[ContentProperty("Text")]
	public abstract class TextControl : CommonControl
	{
		new ITextControl Handler { get { return (ITextControl)base.Handler; } }

		static TextControl()
		{
			EventLookup.Register<TextControl>(c => c.OnTextChanged(null), TextControl.TextChangedEvent);
		}

		protected TextControl(Generator g, Type type, bool initialize = true) : base(g, type, initialize)
		{
		}

		public const string TextChangedEvent = "TextControl.TextChanged";

		public event EventHandler<EventArgs> TextChanged
		{
			add { Properties.AddHandlerEvent(TextChangedEvent, value); }
			remove { Properties.RemoveEvent(TextChangedEvent, value); }
		}

		public virtual void OnTextChanged(EventArgs e)
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
	}
}

