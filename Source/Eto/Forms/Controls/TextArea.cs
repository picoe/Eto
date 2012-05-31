using System;
using System.Collections;

namespace Eto.Forms
{
	public interface ITextArea : ITextControl
	{
		bool ReadOnly { get; set; }
		
		bool Wrap { get; set; }
		
		void Append (string text, bool scrollToCursor);
	}
	
	public class TextArea : TextControl
	{
		ITextArea handler;

		public TextArea ()
			: this (Generator.Current)
		{
		}

		public TextArea (Generator g) : this (g, typeof(ITextArea))
		{
		}
		
		protected TextArea (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ITextArea)base.Handler;
		}
		
		public bool ReadOnly {
			get { return handler.ReadOnly; }
			set { handler.ReadOnly = value; }
		}
		
		public bool Wrap {
			get { return handler.Wrap; }
			set { handler.Wrap = value; }
		}
		
		public void Append (string text, bool scrollToCursor = false)
		{
			handler.Append (text, scrollToCursor);
		}
	}
}
