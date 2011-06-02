using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IForm : IWindow
	{
		void Show ();
	}

	public class Form : Window
	{
		IForm inner;

		public Form () : this(Generator.Current)
		{
		}

		public Form (Generator g) : base(g, typeof(IForm))
		{
			inner = (IForm)this.Handler;
		}

		public void Show ()
		{ 
			OnLoad (EventArgs.Empty);
			inner.Show ();
		}
	}
}
