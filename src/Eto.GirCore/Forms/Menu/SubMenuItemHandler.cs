namespace Eto.GirCore.Forms.Menu
{
	public class SubMenuItemHandler : ButtonMenuItemHandler<SubMenuItem, SubMenuItem.ICallback>, SubMenuItem.IHandler
	{
		protected override bool AlwaysHasSubmenu => true;

		protected override void OnButtonClicked()
		{
			Callback.OnOpening(Widget, EventArgs.Empty);
			base.OnButtonClicked();
		}
	}
}
