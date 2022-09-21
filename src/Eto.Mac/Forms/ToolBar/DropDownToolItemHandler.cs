using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Mac.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<NSMenuToolbarItem, DropDownToolItem>, DropDownToolItem.IHandler
	{
		NSMenu menu;

		protected override bool UseButtonStyle => false;
		protected override bool UseAction => false;

#if MACOS || XAMMAC2
		static readonly IntPtr selInitWithItemIdentifier_Handle = Selector.GetHandle("initWithItemIdentifier:");

		// constructor with identifier isn't available in the version of macOS workload we need yet..
		class EtoMenuToolbarItem : NSMenuToolbarItem
		{
			public EtoMenuToolbarItem()
			{
			}
			
			[Export ("initWithItemIdentifier:")]
			public EtoMenuToolbarItem (string itemIdentifier)
				: base (NSObjectFlag.Empty)
			{
				NSApplication.EnsureUIThread ();
				if (itemIdentifier == null)
					throw new ArgumentNullException ("itemIdentifier");
#if USE_CFSTRING
				var nsitemIdentifier = CFString.CreateNative(itemIdentifier);
#else
				var nsitemIdentifier = NSString.CreateNative(itemIdentifier);
#endif
				
				if (IsDirectBinding) {
					Handle = Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selInitWithItemIdentifier_Handle, nsitemIdentifier);
				} else {
					Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, selInitWithItemIdentifier_Handle, nsitemIdentifier);
				}
				NSString.ReleaseNative (nsitemIdentifier);
				
			}
		}

		protected override NSMenuToolbarItem CreateControl() => new EtoMenuToolbarItem(Identifier);
#else
		protected override NSMenuToolbarItem CreateControl() => new NSMenuToolbarItem(Identifier);
#endif

		public bool ShowDropArrow
		{
			get => Control.ShowsIndicator;
			set => Control.ShowsIndicator = value;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.ShowsIndicator = true;
			menu = new NSMenu();
			menu.AutoEnablesItems = true;
			menu.ShowsStateColumn = true;
			Control.Menu = menu;
			// first item is never shown, it's the "title" of the pull down?? weird.
			menu.InsertItem(new NSMenuItem(string.Empty), 0);
		}

		public void AddMenu(int index, MenuItem item)
		{
			menu.InsertItem((NSMenuItem)item.ControlObject, index + 1);
		}

		public void RemoveMenu(MenuItem item)
		{
			menu.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public void Clear()
		{
			menu.RemoveAllItems();
		}
	}
}
