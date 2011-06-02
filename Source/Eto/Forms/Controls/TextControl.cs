using System;

namespace Eto.Forms
{
	public interface ITextControl : IControl
	{
		string Text { get; set; }
	}
	
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

