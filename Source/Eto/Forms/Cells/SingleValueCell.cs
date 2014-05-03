using System;

namespace Eto.Forms
{
	public abstract class SingleValueCell : Cell
	{
		public IndirectBinding Binding { get; set; }

		protected SingleValueCell()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SingleValueCell (Generator g, Type type, bool initialize)
			: base(g, type, initialize)
		{
		}
	}
}

