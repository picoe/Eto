using System;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Platform.Mac
{
	public interface IMenuActionHandler
	{
		void HandleClick();

		bool Enabled { get; }

		MenuActionItem Widget { get; }
	}

	[Register("EtoMenuActionHandler")]
	public class MenuActionHandler : NSObject
	{
		internal static Selector selActivate = new Selector("activate:");
		WeakReference handler;

		public IMenuActionHandler Handler { get { return (IMenuActionHandler)handler.Target; } set { handler = new WeakReference(value); } }

		[Export("activate:")]
		public void Activate(NSObject sender)
		{
			var h = Handler;
			if (h != null)
				h.HandleClick();
		}

		[Export("validateMenuItem:")]
		public bool ValidateMenuItem(NSMenuItem item)
		{
			var h = Handler;
			if (h != null)
			{
				h.Widget.OnValidate(EventArgs.Empty);
				return h.Enabled;
			}
			return false;
		}
	}
}

