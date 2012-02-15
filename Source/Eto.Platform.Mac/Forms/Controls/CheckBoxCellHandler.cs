using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<NSButtonCell, CheckBoxCell>, ICheckBoxCell
	{
		public CheckBoxCellHandler ()
		{
			Control = new NSButtonCell ();
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
			if (val == null) return new NSNumber((int)NSCellStateValue.Off);
			return base.GetObjectValue (val);
		}
	}
}

