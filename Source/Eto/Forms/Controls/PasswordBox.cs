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
		IPasswordBox inner;

		public PasswordBox() : this(Generator.Current) { }

		public PasswordBox(Generator g)
			: base(g, typeof(IPasswordBox))
		{
			inner = (IPasswordBox)base.Handler;
		}

		public bool ReadOnly
		{
			get { return inner.ReadOnly; }
			set { inner.ReadOnly = value; }
		}

		public virtual int MaxLength
		{
			get { return inner.MaxLength; }
			set { inner.MaxLength = value; }
		}

		public char PasswordChar
		{
			get { return inner.PasswordChar; }
			set { inner.PasswordChar = value; }
		}
	}
}
