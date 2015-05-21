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
		/// This uses the ideal display mode given the state of the application and the owner window that is passed in
		/// </remarks>
		Default = 0,
		/// <summary>
		/// Display the dialog attached to the owner window, if supported (e.g. OS X)
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
	/// A dialog will block user input from the owner form until the dialog is closed.
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
		/// <returns>The result of the modal dialog</returns>
		public new T ShowModal()
		{
			base.ShowModal();
			return Result;
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <returns>The result of the modal dialog</returns>
		public new Task<T> ShowModalAsync()
		{
			return base.ShowModalAsync()
				.ContinueWith(t => Result, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Shows the dialog and blocks until the user closes the dialog
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <returns>The result of the modal dialog</returns>
		/// <param name="owner">The owner control that is showing the form</param>
		public new T ShowModal(Control owner)
		{
			base.ShowModal(owner);
			return Result;
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="owner">The owner control that is showing the form</param>
		public new Task<T> ShowModalAsync(Control owner)
		{
			return base.ShowModalAsync(owner)
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
	/// A dialog will block user input from the owner form until the dialog is closed.
	/// </remarks>
	/// <seealso cref="Form"/>
	/// <seealso cref="Dialog{T}"/>
	[Handler(typeof(Dialog.IHandler))]
	public class Dialog : Window
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		/// Shows the dialog modally, blocking the current thread until it is closed.
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="owner">The owner control that is showing the form</param>
		public void ShowModal(Control owner)
		{
			Owner = owner != null ? owner.ParentWindow : null;
			ShowModal();
		}

		/// <summary>
		/// Shows the dialog modally, blocking the current thread until it is closed.
		/// </summary>
		public void ShowModal()
		{
			bool loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}

			Application.Instance.AddWindow(this);

			Handler.ShowModal(Owner);
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// </remarks>
		/// <param name="owner">The owner control that is showing the form</param>
		public Task ShowModalAsync(Control owner)
		{
			Owner = owner != null ? owner.ParentWindow : null;
			return ShowModalAsync();
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		public Task ShowModalAsync()
		{
			bool loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
			}

			return Handler.ShowModalAsync(Owner);
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
			/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
			/// the dialog is closed.
			/// </remarks>
			/// <param name="owner">The owner control that is showing the form</param>
			void ShowModal(Control owner);

			/// <summary>
			/// Shows the dialog modally asynchronously
			/// </summary>
			/// <remarks>
			/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
			/// the dialog is closed.
			/// </remarks>
			/// <param name="owner">The owner control that is showing the form</param>
			Task ShowModalAsync(Control owner);

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
