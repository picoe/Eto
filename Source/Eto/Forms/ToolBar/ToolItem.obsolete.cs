using System;

namespace Eto.Forms
{
	[Obsolete("Use ButtonToolItem instead")]
	public class ToolBarButton : ButtonToolItem
	{
		public ToolBarButton(Generator generator = null)
			: base(generator)
		{
		}
	}

	[Obsolete("Use CheckToolItem instead")]
	public class CheckToolBarButton : CheckToolItem
	{
		public CheckToolBarButton(Generator generator = null)
			: base(generator)
		{
		}
	}

	[Obsolete("Use SeparatorToolItem instead")]
	public class SeparatorToolBarItem : SeparatorToolItem
	{
		public SeparatorToolBarItem(Generator generator = null)
			: base(generator)
		{
		}
	}
}

