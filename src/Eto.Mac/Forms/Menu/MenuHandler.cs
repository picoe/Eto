using Eto.Forms;
using Eto.Mac.Forms.Actions;
using System;

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
	public interface IMenuHandler
	{
		void SetTopLevel();

		void Activate();
	}

	public class EtoMenu : NSMenu
	{
		public bool WorksWhenModal { get; set; }
	}

	static class MenuHandler
	{
		public static readonly object Enabled_Key = new object();
		public static readonly object WorksWhenModal_Key = new object();
	}

	public abstract class MenuHandler<TControl, TWidget, TCallback> : MacBase<TControl, TWidget, TCallback>, Eto.Forms.Menu.IHandler, IMenuHandler
		where TControl : NSMenuItem
		where TWidget : Eto.Forms.Menu
		where TCallback : Eto.Forms.Menu.ICallback
	{

		public void EnsureSubMenu()
		{
			if (!Control.HasSubmenu)
			{
				Control.Submenu = new NSMenu
				{
					AutoEnablesItems = true,
					ShowsStateColumn = true,
					Title = Control.Title
				};
			}
		}

		public void SetTopLevel()
		{
			EnsureSubMenu();
		}

		public virtual void Activate()
		{
		}

		public virtual void AddMenu(int index, MenuItem item)
		{
			EnsureSubMenu();
			Control.Submenu.InsertItem((NSMenuItem)item.ControlObject, index);
		}

		public virtual void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null)
				return;
			Control.Submenu.RemoveItem((NSMenuItem)item.ControlObject);
			if (Control.Submenu.Count == 0)
				Control.Submenu = null;
		}

		public virtual void Clear()
		{
			Control.Submenu = null;
		}

		public void CreateFromCommand(Command command)
		{
			var m = command as MacCommand;
			if (m != null)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}

		public bool Enabled
		{
			get { return Widget.Properties.Get<bool?>(MenuHandler.Enabled_Key) ?? Control.Enabled; }
			set
			{
				Widget.Properties.Set(MenuHandler.Enabled_Key, (bool?)value);
				Control.Enabled = value;
			}
		}

		public bool Visible
		{
			get => !Control.Hidden;
			set => Control.Hidden = !value;
		}

		public bool WorksWhenModal
		{
			get
			{
				var worksWhenModal = Widget.Properties.Get<bool?>(MenuHandler.WorksWhenModal_Key);
				if (worksWhenModal == null)
				{
					// find top level menu
					var menu = Control.Menu;
					while (menu.Supermenu != null)
						menu = menu.Supermenu;

					worksWhenModal = (menu as EtoMenu)?.WorksWhenModal ?? false;
					Widget.Properties.Set(MenuHandler.WorksWhenModal_Key, worksWhenModal);
				}
				return worksWhenModal.Value;
			}
			set => Widget.Properties.Set(MenuHandler.WorksWhenModal_Key, value);
		}
	}
}
