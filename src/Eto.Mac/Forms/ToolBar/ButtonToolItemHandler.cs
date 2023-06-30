namespace Eto.Mac.Forms.ToolBar
{

	public class ButtonToolItemHandler : ToolItemHandler<NSToolbarItem, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
