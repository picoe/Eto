using System;

namespace Eto.Forms
{
	public interface IToolBarButton : IToolBarActionItem
	{
	}
	
	public class ToolBarButton : ToolBarActionItem
	{

		public event EventHandler<EventArgs> Click;
		
		public ToolBarButton()
			: this((Generator)null)
		{
		}

		public ToolBarButton(Generator generator) : base(generator, typeof(IToolBarButton))
		{
		}

		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

	}


}
