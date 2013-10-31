using System;

namespace Eto.Forms
{
	public interface IForm : IWindow
	{
		void Show();
	}

	public class Form : Window
	{
		new IForm Handler { get { return (IForm)base.Handler; } }

		public Form() : this(Generator.Current)
		{
		}

		public Form(Generator g) : this(g, typeof(IForm))
		{
		}

		protected Form(Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}

		public void Show()
		{ 
			var loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}
			
			Handler.Show();
		}
	}
}
