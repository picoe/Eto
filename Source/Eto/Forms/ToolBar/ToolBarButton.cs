using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolBarButton : IToolBarActionItem
	{
	}
	
	public class ToolBarButton : ToolBarActionItem
	{

		public event EventHandler<EventArgs> Click;
		
		public ToolBarButton(Generator g) : base(g, typeof(IToolBarButton))
		{
		}

		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

	}


}
