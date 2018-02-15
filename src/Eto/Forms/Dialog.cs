using System;
using System.Collections.ObjectModel;
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
		ButtonCollection positiveButtons, negativeButtons;

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
		/// Gets the positive buttons list, these buttons are automatically added to the dialog.
		/// </summary>
		/// <remarks>
		/// Depending on the platform these buttons can be added on the left side or the right
		/// side. The lower the index the closer the button is to the edge.
		/// </remarks>
		/// <value>The positive buttons.</value>
		public Collection<Button> PositiveButtons
		{
			get { return positiveButtons ?? (positiveButtons = new ButtonCollection { Dialog = this, Positive = true }); }
		}

		/// <summary>
		/// Gets the negative buttons list, these buttons are automatically added to the dialog.
		/// </summary>
		/// <remarks>
		/// Depending on the platform these buttons can be added on the left side or the right
		/// side. The lower the index the closer the button is to the edge.
		/// </remarks>
		/// <value>The negative buttons.</value>
		public Collection<Button> NegativeButtons
		{
			get { return negativeButtons ?? (negativeButtons = new ButtonCollection { Dialog = this }); }
		}

		/// <summary>
		/// Shows the dialog modally, blocking the current thread until it is closed.
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// Calling this method is identical to setting the <see cref="Window.Owner"/> property and calling <see cref="ShowModal()"/>.
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
				OnLoadComplete(EventArgs.Empty);
			}

			Application.Instance.AddWindow(this);

			Handler.ShowModal();
		}

		/// <summary>
		/// Shows the dialog modally asynchronously
		/// </summary>
		/// <remarks>
		/// The <paramref name="owner"/> specifies the control on the window that will be blocked from user input until
		/// the dialog is closed.
		/// Calling this method is identical to setting the <see cref="Window.Owner"/> property and calling <see cref="ShowModalAsync()"/>.
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
				OnLoadComplete(EventArgs.Empty);
			}

			Application.Instance.AddWindow(this);

			return Handler.ShowModalAsync();
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
			void ShowModal();

			/// <summary>
			/// Shows the dialog modally asynchronously
			/// </summary>
			Task ShowModalAsync();

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

            /// <summary>
            /// Adds a positive or negative button to the specified position.
            /// </summary>
            /// <param name="positive">Positive or negative button.</param>
            /// <param name="index">Position to add it to,</param>
            /// <param name="item">The button itself.</param>
			void InsertDialogButton(bool positive, int index, Button item);

            /// <summary>
            /// Removes a positive or negative button from the specified position.
            /// </summary>
            /// <param name="positive">Positive or negative button.</param>
            /// <param name="index">Current position of the button.</param>
			/// <param name="item">The button that is being removed.</param>
			void RemoveDialogButton(bool positive, int index, Button item);
		}

		class ButtonCollection : Collection<Button>
		{
			public bool Positive { get; set; }

			public Dialog Dialog { get; set; }

			protected override void InsertItem(int index, Button item)
			{
				base.InsertItem(index, item);
				Dialog.SetParent(item, () => Dialog.Handler.InsertDialogButton(Positive, index, item));
			}

			protected override void RemoveItem(int index)
			{
				var item = Items[index];
				Dialog.RemoveParent(item);
				Dialog.Handler.RemoveDialogButton(Positive, index, item);
				base.RemoveItem(index);
			}

			protected override void SetItem(int index, Button item)
			{
				var oldItem = Items[index];
				Dialog.RemoveParent(oldItem);
				Dialog.Handler.RemoveDialogButton(Positive, index, oldItem);
				base.SetItem(index, item);
				Dialog.SetParent(item, () => Dialog.Handler.InsertDialogButton(Positive, index, item));
			}

			protected override void ClearItems()
			{
				for (int i = Items.Count - 1; i >= 0; i--)
				{
					var item = Items[i];
					Dialog.RemoveParent(item);
					Dialog.Handler.RemoveDialogButton(Positive, i, item);
				}

				base.ClearItems();
			}
		}
	}
}
