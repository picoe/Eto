using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using sd = System.Drawing;

namespace Eto.Platform.Mac
{

	public class ImageMenuItemHandler : MenuHandler<NSMenuItem, ImageMenuItem>, IImageMenuItem, IMenuActionHandler
	{
		Image image;
		bool showImage = ShowImageDefault;

		public static bool ShowImageDefault = false;

		public bool ShowImage
		{
			get { return showImage; }
			set {
				showImage = value;
				SetImage ();
			}
		}

		public ImageMenuItemHandler ()
		{
			Control = new NSMenuItem ();
			Enabled = true;
			Control.Target = new MenuActionHandler{ Handler = this };
			Control.Action = MenuActionHandler.selActivate;
		}
		
		public void HandleClick ()
		{
			Widget.OnClick (EventArgs.Empty);
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case ImageMenuItem.ValidateEvent:
				// handled in MenuActionHandler
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public string Text {
			get	{ return Control.Title; }
			set { 
				Control.SetTitleWithMnemonic (value);
				if (Control.HasSubmenu)
					Control.Submenu.Title = Control.Title;
			}
		}
		
		public string ToolTip {
			get {
				return Control.ToolTip;
			}
			set {
				Control.ToolTip = value;
			}
		}

		public Key Shortcut {
			get { return KeyMap.Convert (Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set { 
				this.Control.KeyEquivalent = KeyMap.KeyEquivalent (value);
				this.Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask (value);
			}
		}

		public Image Image {
			get { return image; }
			set {
				image = value;
				SetImage ();
			}
		}

		void SetImage ()
		{
#if XAMMAC
			// nulls not allowed with XamMac. Remove when XamMac is updated.
			if (this.image != null && ShowImage)
				Control.Image = this.image.ToNS (16);
			else
				Messaging.void_objc_msgSend_IntPtr (Control.Handle, Selector.GetHandle ("setImage:"), IntPtr.Zero);
#else
			Control.Image = ShowImage ? this.image.ToNS (16) : null;
#endif
		}

		public override void AddMenu (int index, MenuItem item)
		{
			base.AddMenu (index, item);
			//if (Control.HasSubmenu) Control.Submenu.Title = Control.Title;
		}

		MenuActionItem IMenuActionHandler.Widget {
			get { return Widget; }
		}

	}
}
