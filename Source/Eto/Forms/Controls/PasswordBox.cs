using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IPasswordBox : ITextControl
	{
		bool ReadOnly { get; set; }

		int MaxLength { get; set; }

		char PasswordChar { get; set; }
	}

	public class PasswordBox : TextControl
	{
		IPasswordBox handler;

		public PasswordBox () : this (Generator.Current)
		{
		}

		public PasswordBox (Generator g)
			: this (g, typeof(IPasswordBox))
		{
		}
		
		protected PasswordBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IPasswordBox)base.Handler;
		}

		public bool ReadOnly {
			get { return handler.ReadOnly; }
			set { handler.ReadOnly = value; }
		}

		public virtual int MaxLength {
			get { return handler.MaxLength; }
			set { handler.MaxLength = value; }
		}

		public char PasswordChar {
			get { return handler.PasswordChar; }
			set { handler.PasswordChar = value; }
		}
	}
}
