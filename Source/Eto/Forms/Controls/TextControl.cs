using System;
using System.Windows.Markup;

namespace Eto.Forms
{
	public interface ITextControl : IControl
	{
		string Text { get; set; }
	}
	
	[ContentProperty("Text")]
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

