using System;

namespace Eto.Forms
{
	public interface ICell
	{
	}
	
	public abstract class Cell : InstanceWidget
	{
		protected Cell()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Cell (Generator g, Type type, bool initialize)
			: base(g, type, initialize)
		{
		}
	}
}

