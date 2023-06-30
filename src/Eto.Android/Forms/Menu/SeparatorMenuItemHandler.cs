using av = Android.Views;

namespace Eto.Android
{
	internal class SeparatorMenuItemHandler : MenuItemHandler<av.IMenuItem, SeparatorMenuItem, SeparatorMenuItem.ICallback>, SeparatorMenuItem.IHandler
	{
		public override void PerformClick()
		{
		}

		public override void CreateControl(av.IMenu androidMenu, System.Int32 index)
		{
			// Actually do nothing, the ContextMenuHandler takes care of separators itself.
		}
	}
}
