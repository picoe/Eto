using System;
using System.Threading.Tasks;

namespace Eto.Forms
{
	/// <summary>
	/// Hint to tell the platform how to display the dialog
	/// </summary>
	/// <remarks>
	/// This tells the platform how you prefer to display the dialog.  Each platform
	/// may support only certain modes and will choose the appropriate mode based on the hint
	/// given.
	/// </remarks>
	[Flags]
	public enum DialogDisplayMode
	{
		/// <summary>
		/// The default display mode for modal dialogs in the platform
		/// </summary>
		/// <remarks>
		/// This uses the ideal display mode given the state of the application and the parent window that is passed in
		/// </remarks>
		Default = 0,
		/// <summary>
		/// Display the dialog attached to the parent window, if supported (e.g. OS X)
		/// </summary>
		Attached = 0x01,
		/// <summary>
		/// Display the dialog as a separate window (e.g. Windows/Linux only supports this mode)
		/// </summary>
		Separate = 0x02,
		/// <summary>
		/// Display in navigation if available
		/// </summary>
		Navigation = 0x04
	}

	/// <summary>
	/// Custom modal dialog with a specified result type
	/// </summary>
	/// <remarks>
	/// This provides a way to show a modal dialog with custom contents to the user.
	/// A dialog will block user input from the parent form until the dialog is closed.
	/// </remarks>
	/// <seealso cref="Dialog"/>
	/// <typeparam name="T">Type result type of the dialog</typeparam>
	public class Dialog<T> : Dialog
	{
		/// <summary>
		/// Gets or sets the result of the dialog
		/// </summary>
		/// <value>The result.</value>
		public T Result { get; set; }

		/// <summary>
		/// Shows the dialog and blocks until the user closes the dialog
		/// </summary>
		/// <remarks>
		/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <returns>The result of the modal dialog</returns>
		/// <param name="parent">Parent control that is showing the form</param>
		public new T ShowModal(Control parent = null)
		{
			base.ShowModal(parent);
			return Result;
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <remarks>
		/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="parent">Parent control that is showing the form</param>
		public new Task<T> ShowModalAsync(Control parent = null)
		{
			return base.ShowModalAsync(parent)
				.ContinueWith(t => Result, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Close the dialog with the specified result
		/// </summary>
		/// <param name="result">Result to return to the caller</param>
		public void Close(T result)
		{
			Result = result;
			Close();
		}
	}

	/// <summary>
	/// Custom modal dialog
	/// </summary>
	/// <remarks>
	/// This provides a way to show a modal dialog with custom contents to the user.
	/// A dialog will block user input from the parent form until the dialog is closed.
	/// </remarks>
	/// <seealso cref="Form"/>
	/// <seealso cref="Dialog{T}"/>
	[Handler(typeof(Dialog.IHandler))]
	public class Dialog : Window
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Dialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Dialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Dialog"/> class.
		/// </summary>
		public Dialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Dialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Dialog(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Gets or sets the display mode hint
		/// </summary>
		/// <value>The display mode.</value>
		public DialogDisplayMode DisplayMode
		{
			get { return Handler.DisplayMode; }
			set { Handler.DisplayMode = value; }
		}

		/// <summary>
		/// Gets or sets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		[Obsolete("This property is deprecated, use Dialog<T> instead to define a custom return type")]
		public DialogResult DialogResult { get; set; }

		/// <summary>
		/// Gets or sets the abort button.
		/// </summary>
		/// <remarks>
		/// On some platforms, the abort button would be called automatically if the user presses the escape key
		/// </remarks>
		/// <value>The abort button.</value>
		public Button AbortButton
		{
			get { return Handler.AbortButton; }
			set { Handler.AbortButton = value; }
		}

		/// <summary>
		/// Gets or sets the default button.
		/// </summary>
		/// <remarks>
		/// On some platforms, the abort button would be called automatically if the user presses the return key
		/// on the form
		/// </remarks>
		/// <value>The default button.</value>
		public Button DefaultButton
		{
			get { return Handler.DefaultButton; }
			set { Handler.DefaultButton = value; }
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <returns>The dialog.</returns>
		/// <param name="parent">Parent.</param>
		[Obsolete("Use ShowModal() instead")]
		public DialogResult ShowDialog(Control parent = null)
		{
			ShowModal(parent);
			return DialogResult;
		}

		/// <summary>
		/// Shows the dialog modally, blocking the current thread until it is closed.
		/// </summary>
		/// <remarks>
		/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="parent">Parent control that is showing the form</param>
		public void ShowModal(Control parent = null)
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
			Handler.ShowModal(parent);
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <remarks>
		/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="parent">Parent control that is showing the form</param>
		public Task ShowModalAsync(Control parent = null)
		{
			var loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}

			return Handler.ShowModalAsync(parent);
		}

		/// <summary>
		/// Close the dialog with the specified dialog result
		/// </summary>
		/// <param name="result">Result.</param>
		[Obsolete("Use Dialog<T> instead to define the return type")]
		public void Close(DialogResult result)
		{
			DialogResult = result;
			Close();
		}

		/// <summary>
		/// Handler interface for the <see cref="Dialog"/> class
		/// </summary>
		public new interface IHandler : Window.IHandler
		{
			/// <summary>
			/// Gets or sets the display mode hint
			/// </summary>
			/// <value>The display mode.</value>
			DialogDisplayMode DisplayMode { get; set; }

			/// <summary>
			/// Shows the dialog modally, blocking the current thread until it is closed.
			/// </summary>
			/// <remarks>
			/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
			/// the dialog is closed.
			/// </remarks>
			/// <param name="parent">Parent control that is showing the form</param>
			void ShowModal(Control parent);

			/// <summary>
			/// Shows the dialog modally asynchronously
			/// </summary>
			/// <remarks>
			/// The <paramref name="parent"/> specifies the control on the window that will be blocked from user input until
			/// the dialog is closed.
			/// </remarks>
			/// <param name="parent">Parent control that is showing the form</param>
			Task ShowModalAsync(Control parent);

			/// <summary>
			/// Gets or sets the default button.
			/// </summary>
			/// <remarks>
			/// On some platforms, the abort button would be called automatically if the user presses the return key
			/// on the form
			/// </remarks>
			/// <value>The default button.</value>
			Button DefaultButton { get; set; }

			/// <summary>
			/// Gets or sets the abort button.
			/// </summary>
			/// <remarks>
			/// On some platforms, the abort button would be called automatically if the user presses the escape key
			/// </remarks>
			/// <value>The abort button.</value>
			Button AbortButton { get; set; }
		}
	}
}
