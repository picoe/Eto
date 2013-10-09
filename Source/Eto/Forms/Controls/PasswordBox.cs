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
		new IPasswordBox Handler { get { return (IPasswordBox)base.Handler; } }

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
		}

		public bool ReadOnly {
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		public virtual int MaxLength {
			get { return Handler.MaxLength; }
			set { Handler.MaxLength = value; }
		}

		public char PasswordChar {
			get { return Handler.PasswordChar; }
			set { Handler.PasswordChar = value; }
		}
	}
}
