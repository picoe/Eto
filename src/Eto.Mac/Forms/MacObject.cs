using System;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if IOS
using Foundation;
#endif

namespace Eto.Mac.Forms
{
	public class MacObject<TControl, TWidget, TCallback> : MacBase<TControl, TWidget, TCallback>
		where TControl: NSObject 
		where TWidget: Widget
	{
		public virtual object EventObject
		{
			get { return Control; }
		}

		/// <summary>
		/// Adds the method dynamically to the specified NSObject.
		/// This uses Objective C to dynamically add the specified method to the class.  Note that you should only pass
		/// objects that have an eto-specific subclass.
		/// 
		/// Typically "v@:[args]"  v = void return, @ = sender 
		/// Arguments:
		/// <list type="unordered">
		///   <item>v - Void</item>
		///   <item>@ - NSObject</item>
		///   <item>B - boolean</item>
		///   <item>d - double</item>
		///   <item>f - float</item>
		///   <item>i - int</item>
		///   <item>{CGSize=dd} - CGSize struct with double values</item>
		///   <item>{CGSize=ff} - CGSize struct with float values</item>
		/// </list>
		/// </summary>
		public new bool AddMethod (IntPtr selector, Delegate action, string arguments, object control = null)
		{
			return base.AddMethod (selector, action, arguments, control ?? EventObject);
		}

		public new bool HasMethod(IntPtr selector, object control = null)
		{
			return base.HasMethod(selector, control ?? EventObject);
		}

		public new NSObject AddObserver (NSString key, Action<ObserverActionEventArgs> action, NSObject control = null)
		{
			return base.AddObserver (key, action, control ?? Control);
		}
		
		public new void AddControlObserver (NSString key, Action<ObserverActionEventArgs> action, NSObject control = null)
		{
			base.AddControlObserver (key, action, control ?? Control);
		}
		
	}
}

