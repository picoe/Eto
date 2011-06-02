using System;
using System.Collections;

namespace Eto.Forms
{
	public interface ITextBox : ITextControl
	{
		bool ReadOnly { get; set; }
	}
	
	public class TextBox : TextControl
	{
		ITextBox inner;
		
		public TextBox() : this(Generator.Current) { }
		
		public TextBox(Generator g) : base(g, typeof(ITextBox))
		{
			inner = (ITextBox)base.Handler;
		}

		public bool ReadOnly
		{
			get { return inner.ReadOnly; }
			set { inner.ReadOnly = value; }
		}
	}
}
