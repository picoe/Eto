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
	public class CheckBoxCellHandler : CellHandler<NSButtonCell, CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{
		public class EtoCell : NSButtonCell, IMacControl
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
		
		public CheckBoxCellHandler ()
		{
			Control = new EtoCell { Handler = this };
			Control.Title = string.Empty;
			Control.SetButtonType (NSButtonType.Switch);
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color.ToNSUI ();
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.BackgroundColor.ToEto ();
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			return Colors.Transparent;
		}


		public override void SetObjectValue (object dataItem, NSObject value)
		{
			if (Widget.Binding != null) {
				var num = value as NSNumber;
				if (num != null) {
					var state = (NSCellStateValue)num.Int32Value;
					bool? boolValue;
					switch (state) {
					default:
						boolValue = null;
						break;
					case NSCellStateValue.On:
						boolValue = true;
						break;
					case NSCellStateValue.Off:
						boolValue = false;
						break;
					}
					Widget.Binding.SetValue(dataItem, boolValue);
				}
			}
		}
		
		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				NSCellStateValue state = NSCellStateValue.Off;
				var val = Widget.Binding.GetValue(dataItem);
				state = val != null ? val.Value ? NSCellStateValue.On : NSCellStateValue.Off : NSCellStateValue.Mixed;
				return new NSNumber((int)state);
			}
			return new NSNumber ((int)NSCellStateValue.Off);
		}
		
		public override nfloat GetPreferredSize (object value, CGSize cellSize, NSCell cell)
		{
			return 25;
		}
	}
}

