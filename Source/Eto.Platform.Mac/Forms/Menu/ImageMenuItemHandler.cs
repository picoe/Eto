using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Mac.Forms.Actions;
using MonoMac.AppKit;
using sd = System.Drawing;

namespace Eto.Platform.Mac
{

	public class ImageMenuItemHandler : MenuHandler<NSMenuItem, ImageMenuItem>, IImageMenuItem, IMenuActionHandler, ICopyFromAction
	{
		Image image;
		bool showImage = ShowImageDefault;

		public static bool ShowImageDefault;

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

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuActionItem.ValidateEvent:
				// handled in MenuActionHandler
				break;
			default:
				base.AttachEvent (id);
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
				Control.ToolTip = value ?? "";
			}
		}

		public Keys Shortcut {
			get { return KeyMap.Convert (Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set { 
				Control.KeyEquivalent = KeyMap.KeyEquivalent (value);
				Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask (value);
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
			Control.Image = ShowImage ? image.ToNS(16) : null;
		}

		MenuActionItem IMenuActionHandler.Widget {
			get { return Widget; }
		}


		public void CopyFrom(CommandBase action)
		{
			var m = action as MacButtonAction;
			if (m != null)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}
	}
}
