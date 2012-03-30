using System;
#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface ITextControl : IControl
	{
		string Text { get; set; }
	}
	
#if DESKTOP
	[ContentProperty("Text")]
#endif
	public class TextControl : Control
	{
		ITextControl inner;
		
		public TextControl(Generator g, Type type) : base(g, type)
		{
			inner = (ITextControl)base.Handler;
		}
		
		public virtual string Text
		{
			get { return inner.Text; }
			set { inner.Text = value; }
		}
	}
}

