using System;

namespace Eto.Forms
{
	/// <summary>
	/// Non-modal form window
	/// </summary>
	/// <seealso cref="Dialog"/>
	[Handler(typeof(Form.IHandler))]
	public class Form : Window
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class.
		/// </summary>
		public Form()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class with the specified <paramref name="generator"/>
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		[Obsolete("Use default constructor instead")]
		public Form(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler interface to create, must implement <see cref="IHandler"/></param>
		/// <param name="initialize">If set to <c>true</c>, initialize the handler after created, otherwise the subclass should initialize</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
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
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}
			
			Handler.Show();
		}

		/// <summary>
		/// Interface handler for the <see cref="Form"/> control
		/// </summary>
		public interface IHandler : Window.IHandler
		{
			/// <summary>
			/// Show the form
			/// </summary>
			void Show();
		}
	}
}
