using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class TextBoxCellHandler : CellHandler<NSTextFieldCell, TextBoxCell>, ITextBoxCell
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
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize, NSCell cell)
		{
			var font = cell.Font ?? NSFont.BoldSystemFontOfSize (NSFont.SystemFontSize);
			var str = new NSString (Convert.ToString (value));
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			return str.StringSize (attrs).Width + 8; // for border
			
		}
	}
}

