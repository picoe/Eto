using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<NSButtonCell, CheckBoxCell>, ICheckBoxCell
	{
		public class EtoCell : NSButtonCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return (object)WeakHandler.Target; }
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
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoCell (ptr) { Handler = this.Handler };
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
			var c = cell as EtoCell;
			c.BackgroundColor = color.ToNS ();
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return c.BackgroundColor.ToEto ();
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			return Colors.Transparent;
		}


		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.Binding != null) {
				var num = val as NSNumber;
				if (num != null) {
					var state = (NSCellStateValue)num.IntValue;
					bool? value;
					switch (state) {
					default:
					case NSCellStateValue.Mixed:
						value = null;
						break;
					case NSCellStateValue.On:
						value = true;
						break;
					case NSCellStateValue.Off:
						value = false;
						break;
					}
					Widget.Binding.SetValue(dataItem, value);
				}
			}
		}
		
		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				NSCellStateValue state = NSCellStateValue.Off;
				var val = Widget.Binding.GetValue(dataItem);
				if (val is bool?) {
					var boolVal = (bool?)val;
					state = boolVal != null ? boolVal.Value ? NSCellStateValue.On : NSCellStateValue.Off : NSCellStateValue.Mixed;
				}
				else if (val is bool) {
					var boolVal = (bool)val;
					state = boolVal ? NSCellStateValue.On : NSCellStateValue.Off;
				}
				return new NSNumber((int)state);
			}
			return new NSNumber ((int)NSCellStateValue.Off);
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize, NSCell cell)
		{
			return 25;
		}
	}
}

