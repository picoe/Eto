using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TextBoxCellHandler : CellHandler<NSTextFieldCell, TextBoxCell>, ITextBoxCell
	{
		public class EtoCell : NSTextFieldCell, IMacControl
		{
			public object Handler { get; set; }
			
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
		
		public TextBoxCellHandler ()
		{
			Control = new EtoCell { Handler = this };
			Control.Wraps = false;
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.BackgroundColor = Generator.ConvertNS (color);
			c.DrawsBackground = color != Color.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return Generator.Convert (c.BackgroundColor);
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.TextColor = Generator.ConvertNS (color);
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return Generator.Convert (c.TextColor);
		}

		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				return val is string ? new NSString((string)val) : null;
			}
			else
				return new NSString ();
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.Binding != null) {
				var str = val as NSString;
				if (str != null)
					Widget.Binding.SetValue (dataItem, (string)str);
				else
					Widget.Binding.SetValue (dataItem, null);
			}
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var font = Control.Font ?? NSFont.SystemFontOfSize (NSFont.SystemFontSize);
			var str = new NSString (Convert.ToString (value));
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			return str.StringSize (attrs).Width + 4; // for border
			
		}
	}
}

