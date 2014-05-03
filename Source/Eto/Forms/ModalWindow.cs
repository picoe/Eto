using System;

namespace Eto.Forms
{
	public interface IModalWindow : IWindow
	{
		ModalWindowDisplayMode DisplayMode { get; set; }

		void ShowModal(Action completed, Control parent);

		Button DefaultButton { get; set; }

		Button AbortButton { get; set; }
	}

	/// <summary>
	/// Hint to tell the platform how to display the modal window
	/// </summary>
	/// <remarks>
	/// This tells the platform how you prefer to display the dialog.  Each platform
	/// may support only certain modes and will choose the appropriate mode based on the hint
	/// given.
	/// </remarks>
	public enum ModalWindowDisplayMode
	{
		/// <summary>
		/// The default display mode for modal dialogs in the platform
		/// </summary>
		Default,
		/// <summary>
		/// Display the dialog attached to the parent window, if supported (e.g. OS X)
		/// </summary>
		Attached,
		/// <summary>
		/// Display the dialog as a separate window (e.g. Windows/Linux only supports this mode)
		/// </summary>
		Separate
	}

	/// <summary>
	/// A modal window is one that captures the focus until it is dismissed.
	/// On Windows, it is implemented using a modal dialog.
	/// On iOS, it is implemented using a modal view controller.
	/// </summary>
	[Handler(typeof(IModalWindow))]
	public class ModalWindow : Window
	{
		new IModalWindow Handler { get { return (IModalWindow)base.Handler; } }

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ModalWindow(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public ModalWindow()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ModalWindow(Generator generator) : this(generator, typeof(IModalWindow))
		{
		}

		public ModalWindowDisplayMode DisplayMode
		{
			get { return Handler.DisplayMode; }
			set { Handler.DisplayMode = value; }
		}

		public Button AbortButton
		{
			get { return Handler.AbortButton; }
			set { Handler.AbortButton = value; }
		}

		public Button DefaultButton
		{
			get { return Handler.DefaultButton; }
			set { Handler.DefaultButton = value; }
		}

		public void ShowModal(Action completed, Control parent = null)
		{
			var loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}
			
			Handler.ShowModal(completed, parent);
		}
	}
}
