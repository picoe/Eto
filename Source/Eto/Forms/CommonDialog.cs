using System;

namespace Eto.Forms
{
	public interface ICommonDialog : IWidget
	{
		DialogResult ShowDialog (Window parent);
	}
	
	public abstract class CommonDialog : Widget
	{
		new ICommonDialog Handler { get { return (ICommonDialog)base.Handler; } }

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
		
	}
}

