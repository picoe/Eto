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
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class with the specified <paramref name="handler"/>
		/// </summary>
		/// <param name="handler">Handler to use as the implementation of the form.</param>
		public Form(IHandler handler)
			: base(handler)
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

			Application.Instance.AddWindow(this);
			Handler.Show();
		}

		/// <summary>
		/// Interface handler for the <see cref="Form"/> control
		/// </summary>
		public new interface IHandler : Window.IHandler
		{
			/// <summary>
			/// Show the form
			/// </summary>
			void Show();
		}
	}
}
