using System;
#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface ITextControl : ICommonControl
	{
		string Text { get; set; }
	}
	
#if DESKTOP
	[ContentProperty("Text")]
#endif
	public class TextControl : CommonControl
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

