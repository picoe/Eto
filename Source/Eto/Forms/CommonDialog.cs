using System;

namespace Eto.Forms
{
	public abstract class CommonDialog : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		protected CommonDialog()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CommonDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}

		public DialogResult ShowDialog (Control parent)
		{
			return ShowDialog (parent.ParentWindow);
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			return Handler.ShowDialog (parent);
		}
		
		public new interface IHandler : Widget.IHandler
		{
			DialogResult ShowDialog (Window parent);
		}

	}
}

