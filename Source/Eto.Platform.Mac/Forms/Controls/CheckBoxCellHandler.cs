using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<NSButtonCell, CheckBoxCell>, ICheckBoxCell
	{
		public class EtoButtonCell : NSButtonCell, IMacControl
		{
			public object Handler { get; set; }

			public EtoButtonCell ()
			{
			}

			public EtoButtonCell (IntPtr handle) : base(handle)
			{
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoButtonCell (ptr) { Handler = this.Handler };
			}
		}
		
		public CheckBoxCellHandler ()
		{
			Control = new EtoButtonCell { Handler = this };
			Control.Title = string.Empty;
			Control.SetButtonType (NSButtonType.Switch);
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.Binding != null) {
				var num = val as NSNumber;
				if (num != null) {
					var state = (NSCellStateValue)num.IntValue;
					bool? value;
					switch (state) {
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
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			return 25;
		}
	}
}

