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
		ITextControl handler;
		
		protected TextControl(Generator g, Type type, bool initialize = true) : base(g, type, initialize)
		{
			handler = (ITextControl)base.Handler;
		}
		
		public virtual string Text
		{
			get { return handler.Text; }
			set { handler.Text = value; }
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

