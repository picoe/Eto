using System;

namespace Eto.Forms
{
	public interface ICommonDialog : IInstanceWidget
	{
		DialogResult ShowDialog (Window parent);
	}
	
	public abstract class CommonDialog : InstanceWidget
	{
		ICommonDialog handler;
		
		protected CommonDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (ICommonDialog)Handler;
		}

		public DialogResult ShowDialog (Control parent)
		{
			return ShowDialog (parent.ParentWindow);
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			return handler.ShowDialog (parent);
		}
		
	}
}

