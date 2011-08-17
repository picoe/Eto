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
		IDialog inner;

		protected Dialog (Generator g, Type type)
			: base(g, type)
		{
			inner = (IDialog)this.Handler;
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
			get { return inner.AbortButton; }
			set { inner.AbortButton = value; }
		}
		
		public Button DefaultButton
		{
			get { return inner.DefaultButton; }
			set { inner.DefaultButton = value; }
		}

		public DialogResult ShowDialog (Control parent)
		{
			OnLoad (EventArgs.Empty);
			this.DialogResult = inner.ShowDialog (parent);
			return DialogResult;
		}
	}
}
