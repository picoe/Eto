using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{

	public class ImageMenuItemHandler : MenuHandler<NSMenuItem, ImageMenuItem>, IImageMenuItem, IMenuActionHandler
	{
		Icon icon;
		
		public static bool UseImages = false;

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
		
		
		#region IMenuItem Members
		
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

		public Icon Icon {
			get { return icon; }
			set {
				icon = value;
				if (UseImages) {
					if (icon != null) {
						var image = ((NSImage)icon.ControlObject);
						var rep = image.BestRepresentation (new System.Drawing.RectangleF (0, 0, 16, 16), null, new NSDictionary ());
						var image2 = new NSImage ();
						image2.AddRepresentation (rep);
						Control.Image = image2;
					} else
						Control.Image = null;
				}
			}
		}

		#endregion
		
		public override void AddMenu (int index, MenuItem item)
		{
			base.AddMenu (index, item);
			//if (Control.HasSubmenu) Control.Submenu.Title = Control.Title;
		}


	}
}
