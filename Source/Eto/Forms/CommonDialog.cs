using System;

namespace Eto.Forms
{
	public interface ICommonDialog : IWidget
	{
		DialogResult ShowDialog(Window parent);

	}
	
	public abstract class CommonDialog : Widget
	{
		ICommonDialog inner;
		
		public CommonDialog (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			inner = (ICommonDialog)Handler;
		}
		
		public DialogResult ShowDialog(Window parent)
		{
			return inner.ShowDialog(parent);
		}
		
	}
}

