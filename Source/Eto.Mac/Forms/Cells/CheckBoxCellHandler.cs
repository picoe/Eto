using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
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
					var state = (NSCellStateValue)num.IntValue;
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
		
		public override float GetPreferredSize (object value, NSSize cellSize, NSCell cell)
		{
			return 25;
		}
	}
}

