using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using MonoMac.CoreGraphics;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ComboBoxCellHandler : CellHandler<NSPopUpButtonCell, ComboBoxCell>, IComboBoxCell
	{
		IListStore dataStore;
		
		public class EtoCell : NSPopUpButtonCell, IMacControl
		{
			public object Handler { get; set; }

			public EtoCell ()
			{
			}

			public EtoCell (IntPtr handle) : base(handle)
			{
			}

			public NSColor TextColor { get; set; }

			public bool DrawsBackground { get; set; }

			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoCell (ptr) { Handler = this.Handler };
			}

			public override void DrawBorderAndBackground (System.Drawing.RectangleF cellFrame, NSView controlView)
			{
				if (DrawsBackground) {
					var nscontext = NSGraphicsContext.CurrentContext;
					var context = nscontext.GraphicsPort;

					context.SetFillColor (BackgroundColor.ToCG ());
					context.FillRect (cellFrame);
				}

				base.DrawBorderAndBackground (cellFrame, controlView);
			}

			public override System.Drawing.RectangleF DrawTitle (NSAttributedString title, System.Drawing.RectangleF frame, NSView controlView)
			{
				if (TextColor != null) {
					var newtitle = title.MutableCopy () as NSMutableAttributedString;
					var range = new NSRange (0, title.Length);
					newtitle.RemoveAttribute (NSAttributedString.ForegroundColorAttributeName, range);
					newtitle.AddAttribute (NSAttributedString.ForegroundColorAttributeName, TextColor, range);
					title = newtitle;
				}
				var rect = base.DrawTitle (title, frame, controlView);
				return rect;
			}
		}
		
		public ComboBoxCellHandler ()
		{
			Control = new EtoCell { Handler = this, ControlSize = NSControlSize.Regular, Bordered = false };
			Control.Title = string.Empty;
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.BackgroundColor = color.ToNS ();
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return c.BackgroundColor.ToEto ();
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.TextColor = color.ToNS ();
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return c.TextColor.ToEto ();
		}

		IEnumerable<IListItem> GetItems ()
		{
			if (dataStore == null)
				yield break;
			for (int i = 0; i < dataStore.Count; i ++) {
				var item = dataStore [i];
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
				var item = dataStore [row];
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
	
				return new NSNumber (found);
			}
			return null;
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize, NSCell cell)
		{
			return 100;
		}
	}
}

