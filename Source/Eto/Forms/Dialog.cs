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

	public interface IDialog : IWindow
	{
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
	}
}
