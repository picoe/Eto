using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ComboBoxCellHandler : CellHandler<NSPopUpButtonCell, ComboBoxCell>, IComboBoxCell
	{
		IListStore dataStore;
		
		public class EtoPopUpButtonCell : NSPopUpButtonCell, IMacControl
		{
			public object Handler { get; set; }

			public EtoPopUpButtonCell ()
			{
			}

			public EtoPopUpButtonCell (IntPtr handle) : base(handle)
			{
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoPopUpButtonCell (ptr) { Handler = this.Handler };
			}
		}
		
		public ComboBoxCellHandler ()
		{
			Control = new EtoPopUpButtonCell { Handler = this, ControlSize = NSControlSize.Mini };
			Control.Title = string.Empty;
		}
		
		IEnumerable<IListItem> GetItems ()
		{
			if (dataStore == null)
				yield break;
			for (int i = 0; i < dataStore.Count; i ++) {
				var item = dataStore[i];
				yield return item;
			}
		}

		public IListStore DataStore {
			get { return dataStore; }
			set {
				dataStore = value;
				this.Control.RemoveAllItems ();
				this.Control.AddItems (GetItems ().Select (r => r.Text).ToArray ());
			}
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.Binding != null) {
				var row = ((NSNumber)val).Int32Value;
				var item = dataStore[row];
				var value = item != null ? item.Key : null;
				Widget.Binding.SetValue (dataItem, value);
			}
		}
		
		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				var key = Convert.ToString (val);
				int found = -1;
				int index = 0;
				foreach (var item in GetItems ()) {
					if (object.Equals (item.Key, key)) {
						found = index;
						break;
					}
					index ++;
				}
	
				return new NSNumber(found);
			}
			return null;
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			return 100;
		}
	}
}

