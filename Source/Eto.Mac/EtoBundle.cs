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

namespace Eto.Mac
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
			var str = Messaging.GetNSObject<NSString>(filePath);
			return str == null || str.Length == 0 || Messaging.bool_objc_msgSend_IntPtr_IntPtr(self, selEtoLoadNibNamed, filePath, owner);
		}
	}
}

