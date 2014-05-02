using System;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{
	public static class EtoBundle
	{
		
		static readonly IntPtr selEtoLoadNibNamed = Selector.GetHandle ("etoLoadNibNamed:owner:");
		static readonly IntPtr selLoadNibNamed = Selector.GetHandle ("loadNibNamed:owner:");
		
		public static void Init ()
		{
			var bundleClass = ObjCExtensions.GetMetaClass ("NSBundle");
			bundleClass.AddMethod (selEtoLoadNibNamed, new Func<IntPtr, IntPtr, IntPtr, IntPtr,bool> (EtoLoadNibNamed), "B@:@@");
			bundleClass.ExchangeMethod (selLoadNibNamed, selEtoLoadNibNamed);
		}
		
		static bool EtoLoadNibNamed (IntPtr self, IntPtr sel, IntPtr filePath, IntPtr owner)
		{
			var str = new NSString (filePath);
			return str.Length == 0 || Messaging.bool_objc_msgSend_IntPtr_IntPtr(self, selEtoLoadNibNamed, filePath, owner);
		}
	}
}

