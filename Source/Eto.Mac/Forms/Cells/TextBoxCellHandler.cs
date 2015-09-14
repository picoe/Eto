using System;
using Eto.Forms;
using Eto.Drawing;

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
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Cells
{
	public class TextBoxCellHandler : CellHandler<NSTextFieldCell, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler
	{
		public class EtoCell : NSTextFieldCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell ()
			{
			}
			
			public EtoCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.CopyWithZoneHandle, zone);
				return new EtoCell (ptr) { Handler = Handler };
			}
		}
		
		public TextBoxCellHandler ()
		{
			Control = new EtoCell { 
				Handler = this,
				UsesSingleLineMode = true,
				Wraps = false,
				Scrollable = true
			};
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color.ToNSUI ();
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.BackgroundColor.ToEto ();
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.TextColor = color.ToNSUI ();
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.TextColor.ToEto ();
		}

		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				return val != null ? new NSString(Convert.ToString(val)) : null;
			}
			return null;
		}
		
		public override void SetObjectValue (object dataItem, NSObject value)
		{
			if (Widget.Binding != null) {
				var str = value as NSString;
				if (str != null)
					Widget.Binding.SetValue (dataItem, str.ToString());
				else
					Widget.Binding.SetValue (dataItem, null);
			}
		}
		
		public override nfloat GetPreferredSize (object value, CGSize cellSize, NSCell cell)
		{
			var font = cell.Font ?? NSFont.BoldSystemFontOfSize (NSFont.SystemFontSize);
			var str = new NSString (Convert.ToString (value));
			var attrs = NSDictionary.FromObjectAndKey (font, NSStringAttributeKey.Font);
			return (float)str.StringSize (attrs).Width + 8; // for border
			
		}
	}
}

