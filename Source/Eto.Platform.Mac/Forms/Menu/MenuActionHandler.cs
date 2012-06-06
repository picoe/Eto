using System;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Platform.Mac
{
	public interface IMenuActionHandler
	{
		void HandleClick ();
		
		bool Enabled { get; }
		
		MenuActionItem Widget { get; }
	}
		
	[Register("EtoMenuActionHandler")]
	public class MenuActionHandler : NSObject
	{
		internal static Selector selActivate = new Selector ("activate:");
		
		public IMenuActionHandler Handler { get; set; }
			
		[Export("activate:")]
		public void Activate (NSObject sender)
		{
			Handler.HandleClick ();
		}
			
		[Export("validateMenuItem:")]
		public bool ValidateMenuItem (NSMenuItem item)
		{
			Handler.Widget.OnValidate(EventArgs.Empty);
			return Handler.Enabled;
		}
	}
}

