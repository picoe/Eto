using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class TextAreaHandler : MacView<NSScrollView, TextArea>, ITextArea
	{
		NSTextView text;
		
		public TextAreaHandler()
		{
			Control = new NSScrollView();
			Control.AutoresizesSubviews = true;
			Control.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
			Control.HasVerticalScroller = true;
			Control.HasHorizontalScroller = true;
			Control.AutohidesScrollers = true;
			Control.BorderType = NSBorderType.BezelBorder;
			
			text = new NSTextView();
			//text.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
			//text.TextContainer.ContainerSize = new System.Drawing.SizeF(1.0e7f, 1.0e7f);
			//text.TextContainer.WidthTracksTextView = false;
			text.HorizontallyResizable = true;
			text.VerticallyResizable = true;
			text.Editable = true;
			text.Selectable = true;
			text.TextDidChange += delegate {
				Widget.OnTextChanged (EventArgs.Empty);
			};
			
			Control.DocumentView = text;
		}
		
		#region ITextArea Members
		
		public bool ReadOnly
		{
			get { return text.Editable; }
			set { text.Editable = value; }
		}
		
		public string Text {
			get {
				return text.Value;
			}
			set {
				text.Value = value;
			}
		}
		
		#endregion
	}
}
