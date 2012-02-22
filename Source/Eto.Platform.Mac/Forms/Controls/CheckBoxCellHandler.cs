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
		
		public override object SetObjectValue (MonoMac.Foundation.NSObject val)
		{
			var num = val as NSNumber;
			if (num != null) {
				var state = (NSCellStateValue)num.IntValue;
				return state == NSCellStateValue.On;
			}
			return false;
		}
		
		public override NSObject GetObjectValue (object val)
		{
			if (val == null)
				return new NSNumber ((int)NSCellStateValue.Off);
			return base.GetObjectValue (val);
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			return 25;
		}
	}
}

