using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms.Menu;

namespace Eto.Mac.Forms.ToolBar
{
	/// <summary>
	/// Drop down handler pre-catalina.  This could probably be done a little better.
	/// </summary>
	public class DropDownToolItemPreCatalinaHandler : ToolItemHandler<NSToolbarItem, DropDownToolItem>, DropDownToolItem.IHandler
	{
		ContextMenu contextMenu;
		MenuSegmentedItem menuItem;
		SegmentedButton segmentedButton;
		bool showDropArrow = true;
		protected override bool UseButtonStyle => false;

		protected override bool IsButton => true;

		protected override bool UseAction => false;

		public bool ShowDropArrow
		{
			get => showDropArrow;
			set
			{
				showDropArrow = value;
				if (SegmentedButtonHandler.supportsMenuIndicator)
				{
					var nssegmentedControl = SegmentedButtonHandler.GetControl(segmentedButton);
					nssegmentedControl.SetShowsMenuIndicator(value, 0);
				}
			}
		}

		protected override void Initialize()
		{
			contextMenu = new ContextMenu();

			menuItem = new MenuSegmentedItem { Menu = contextMenu };
			
			segmentedButton = new SegmentedButton();
			segmentedButton.Items.Add(menuItem);
			
			Control.View = segmentedButton.ToNative(true);

			base.Initialize();
		}

		protected override NSMenuItem CreateMenuFormRepresentation()
		{
			var menu = base.CreateMenuFormRepresentation();
			menu.Submenu = contextMenu.ToNS();
			return menu;
		}

		public void AddMenu(int index, MenuItem item)
		{
			contextMenu.Items.Insert(index, item);
		}

		public void RemoveMenu(MenuItem item)
		{
			contextMenu.Items.Remove(item);
		}

		public void Clear()
		{
			contextMenu.Items.Clear();
		}

		protected override void SetImage()
		{
			base.SetImage();
			if (Image is Bitmap bmp)
				menuItem.Image = bmp.WithSize(ImageSize, ImageSize);
			else if (Image is Icon icon)
				menuItem.Image = icon.WithSize(ImageSize, ImageSize);
		}

	}
}