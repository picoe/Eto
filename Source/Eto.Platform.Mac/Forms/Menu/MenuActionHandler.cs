using System;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public interface IMenuActionHandler
	{
		void HandleClick ();
		
		bool Enabled { get; }
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
			return Handler.Enabled;
		}
	}
}

