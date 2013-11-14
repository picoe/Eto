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

		public Form()
			: this((Generator)null)
		{
		}

		public Form(Generator generator) : this(generator, typeof(IForm))
		{
		}

		protected Form(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
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
