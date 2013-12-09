using System;

namespace Eto.Forms
{
	public interface IToolBarButton : IToolBarActionItem
	{
	}
	
	public class ToolBarButton : ToolBarActionItem
	{
		public ToolBarButton()
			: this((Generator)null)
		{
		}

		public ToolBarButton(Generator generator) : base(generator, typeof(IToolBarButton))
		{
		}
	}
}
