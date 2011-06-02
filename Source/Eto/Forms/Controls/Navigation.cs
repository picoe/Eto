using System;

namespace Eto.Forms
{
	public interface INavigation : IControl
	{
		void Push(Control control);
		void Pop();
	}
	
	public class Navigation : Control
	{
		INavigation inner;

		public Navigation ()
			: this(Generator.Current)
		{
		}
		
		public Navigation (Generator g)
			: base(g, typeof(INavigation))
		{
			inner = (INavigation)Handler;
		}
		
		public Navigation(Control control)
			: this()
		{
			Push(control);
		}
		
		public virtual void Push(Control control)
		{
			inner.Push(control);
		}
		
		public virtual void Pop()
		{
			inner.Pop();
		}
	}
}

