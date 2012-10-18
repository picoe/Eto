using System;
using Eto.Drawing;

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
	public enum DialogDisplayMode
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

	public interface IDialog : IWindow
	{
		DialogDisplayMode DisplayMode { get; set; }
		DialogResult ShowDialog (Control parent);
		Button DefaultButton { get; set; }
		Button AbortButton { get; set; }
	}

	public class Dialog : Window
	{
		IDialog handler;

		protected Dialog (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			handler = (IDialog)this.Handler;
			this.DialogResult = DialogResult.None;
		}

		public Dialog () : this(Generator.Current)
		{
		}

		public Dialog (Generator g) : this(g, typeof(IDialog))
		{
		}

		public DialogDisplayMode DisplayMode
		{
			get { return handler.DisplayMode; }
			set { handler.DisplayMode = value; }
		}

		public DialogResult DialogResult { get; set; }
		
		public Button AbortButton
		{
			get { return handler.AbortButton; }
			set { handler.AbortButton = value; }
		}
		
		public Button DefaultButton
		{
			get { return handler.DefaultButton; }
			set { handler.DefaultButton = value; }
		}

		public DialogResult ShowDialog (Control parent)
		{
			var loaded = Loaded;
			if (!loaded) {
				OnPreLoad (EventArgs.Empty);
				OnLoad (EventArgs.Empty);
				OnLoadComplete (EventArgs.Empty);
			}
			
			this.DialogResult = handler.ShowDialog (parent);
			return DialogResult;
		}

		public void Close (DialogResult result)
		{
			this.DialogResult = result;
			Close ();
		}
	}
}
