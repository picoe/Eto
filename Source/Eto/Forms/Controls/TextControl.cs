using System;

#if XAML
using System.Windows.Markup;
#endif
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

		protected TextControl(Generator g, Type type, bool initialize = true) : base(g, type, initialize)
		{
		}

		public const string TextChangedEvent = "Control.TextChanged";
		EventHandler<EventArgs> textChanged;

		public event EventHandler<EventArgs> TextChanged
		{
			add
			{
				HandleEvent(TextChangedEvent);
				textChanged += value;
			}
			remove { textChanged -= value; }
		}

		public virtual void OnTextChanged(EventArgs e)
		{
			if (textChanged != null)
				textChanged(this, e);
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

