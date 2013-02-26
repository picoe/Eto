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
		IForm handler;

		public Form () : this(Generator.Current)
		{
		}

		public Form (Generator g) : this(g, typeof(IForm))
		{
		}
		
		protected Form (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (IForm)this.Handler;
		}

		public void Show ()
		{ 
			var loaded = Loaded;
			if (!loaded) {
				OnPreLoad (EventArgs.Empty);
				OnLoad (EventArgs.Empty);
				OnLoadComplete (EventArgs.Empty);
			}
			
			handler.Show ();
		}
	}
}
