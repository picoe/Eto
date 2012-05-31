using System;
using System.Collections;

namespace Eto.Forms
{
	public interface ITextBox : ITextControl
	{
		bool ReadOnly { get; set; }

		int MaxLength { get; set; }
		
		string PlaceholderText { get; set; }
	}
	
	public class TextBox : TextControl
	{
		ITextBox handler;
		
		public TextBox () : this(Generator.Current)
		{
		}
		
		public TextBox (Generator g) 
			: this(g, typeof(ITextBox))
		{
			
		}
		protected TextBox (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (ITextBox)base.Handler;
		}

		public bool ReadOnly {
			get { return handler.ReadOnly; }
			set { handler.ReadOnly = value; }
		}
		
		public int MaxLength {
			get { return handler.MaxLength; }
			set { handler.MaxLength = value; }
		}
		
		public string PlaceholderText {
			get { return handler.PlaceholderText; }
			set { handler.PlaceholderText = value; }
		}
	}
}
