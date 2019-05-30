using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Actions;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Menu
{
	public class ButtonMenuItemHandler : MenuHandler<NSMenuItem, ButtonMenuItem, ButtonMenuItem.ICallback>, ButtonMenuItem.IHandler, IMenuActionHandler
	{
		Image image;
		string text;
		bool showImage = ShowImageDefault;
		public static bool ShowImageDefault;

		public bool ShowImage
		{
			get { return showImage; }
			set
			{
				showImage = value;
				SetImage();
			}
		}

		protected override NSMenuItem CreateControl()
		{
			return new NSMenuItem();
		}

		protected override void Initialize()
		{
			Enabled = true;
			Control.Target = new MenuActionHandler{ Handler = this };
			Control.Action = MenuActionHandler.selActivate;
			base.Initialize();
		}

		public override void Activate()
		{
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case MenuItem.ValidateEvent:
				// handled in MenuActionHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string Text
		{
			get	{ return text; }
			set
			{ 
				text = value;

				Control.Title = MacConversions.StripAmpersands(value ?? string.Empty);
				if (Control.HasSubmenu)
					Control.Submenu.Title = Control.Title;
			}
		}

		public string ToolTip
		{
			get { return Control.ToolTip; }
			set { Control.ToolTip = value ?? string.Empty; }
		}

		public Keys Shortcut
		{
			get { return KeyMap.Convert(Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set
			{ 
				Control.KeyEquivalent = KeyMap.KeyEquivalent(value);
				Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask(value);
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetImage();
			}
		}

		void SetImage()
		{
			Control.Image = ShowImage ? image.ToNS(16) : null;
		}

		MenuItem IMenuActionHandler.Widget
		{
			get { return Widget; }
		}
	}
}
