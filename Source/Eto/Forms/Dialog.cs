using System;
using System.Threading.Tasks;

namespace Eto.Forms
{
	public enum DialogResult
	{
		None,
		Ok,
		Cancel,
		Yes,
		No,
		Abort,
		Ignore,
		Retry
	}

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

	public interface IDialog : IWindow
	{
		DialogDisplayMode DisplayMode { get; set; }

		void ShowModal(Control parent);
		Task ShowModalAsync(Control parent);

		Button DefaultButton { get; set; }

		Button AbortButton { get; set; }
	}

	public class Dialog<T> : Dialog
	{
		public T Result { get; set; }

		public new T ShowModal(Control parent = null)
		{
			base.ShowModal(parent);
			return Result;
		}

		public new Task<T> ShowModalAsync(Control parent = null)
		{
			return base.ShowModalAsync(parent)
				.ContinueWith(t => Result, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		public void Close(T result)
		{
			Result = result;
			Close();
		}
	}

	[Handler(typeof(IDialog))]
	public class Dialog : Window
	{
		new IDialog Handler { get { return (IDialog)base.Handler; } }

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Dialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public Dialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Dialog(Generator generator) : this(generator, typeof(IDialog))
		{
		}

		public DialogDisplayMode DisplayMode
		{
			get { return Handler.DisplayMode; }
			set { Handler.DisplayMode = value; }
		}

		[Obsolete("This property is deprecated, use Dialog<T> instead to define a custom return type")]
		public DialogResult DialogResult { get; set; }

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

		[Obsolete("Use ShowModal() instead")]
		public DialogResult ShowDialog(Control parent = null)
		{
			ShowModal(parent);
			return DialogResult;
		}

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

			Handler.ShowModal(parent);
		}

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

		[Obsolete("Use Dialog<T> instead to define the return type")]
		public void Close(DialogResult result)
		{
			DialogResult = result;
			Close();
		}
	}
}
