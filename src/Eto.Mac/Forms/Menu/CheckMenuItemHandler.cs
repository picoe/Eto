using System;
using Eto.Forms;

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
	public class CheckMenuItemHandler : MenuHandler<NSMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler, IMenuActionHandler
	{
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
			Checked = !Checked;
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		#region IMenuItem Members

		public string Text
		{
			get	{ return Control.Title; }
			set { Control.Title = MacConversions.StripAmpersands(value ?? string.Empty); }
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

		public bool Checked
		{
			get { return Control.State == NSCellStateValue.On; }
			set
			{ 
				if (Checked != value)
				{
					Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; 
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		#endregion

		MenuItem IMenuActionHandler.Widget
		{
			get { return Widget; }
		}

		MenuItem.ICallback IMenuActionHandler.Callback
		{
			get { return Callback; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case CheckMenuItem.CheckedChangedEvent:
					break;
				case MenuItem.ValidateEvent:
					// handled in MenuActionHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
