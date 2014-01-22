using System;

namespace Eto.Forms
{
	/// <summary>
	/// Interface handler for the <see cref="Form"/> control
	/// </summary>
	public interface IForm : IWindow
	{
		/// <summary>
		/// Show the form
		/// </summary>
		void Show();
	}

	/// <summary>
	/// Non-modal form window
	/// </summary>
	/// <seealso cref="Dialog"/>
	public class Form : Window
	{
		new IForm Handler { get { return (IForm)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class.
		/// </summary>
		public Form()
			: this((Generator)null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class with the specified <paramref name="generator"/>
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		public Form(Generator generator) : this(generator, typeof(IForm))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler interface to create, must implement <see cref="IForm"/></param>
		/// <param name="initialize">If set to <c>true</c>, initialize the handler after created, otherwise the subclass should initialize</param>
		protected Form(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// Show the form
		/// </summary>
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
