using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using Eto.Platform.Mac.Drawing;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.CoreText;
using System.Text.RegularExpressions;
using System.Linq;

namespace Eto.Platform.Mac
{
	public class LabelHandler : MacView<LabelHandler.EtoLabel, Label>, ILabel
	{
		Font font;
		bool is106;
		
		class MyTextFieldCell : NSTextFieldCell
		{
			public LabelHandler Handler { get; set; }
			
			public MyTextFieldCell ()
			{
			}

			public override SD.RectangleF TitleRectForBounds (SD.RectangleF theRect)
			{
				SD.RectangleF titleFrame = base.TitleRectForBounds (theRect);
				var titleSize = this.CellSizeForBounds (theRect);

				switch (Handler.VerticalAlign) {
				case VerticalAlign.Middle:
					titleFrame.Y = theRect.Y + (theRect.Height - titleSize.Height) / 2.0F;
					break;
				case VerticalAlign.Top:
					// do nothing!
					break;
				case VerticalAlign.Bottom:
					titleFrame.Y = theRect.Y + (theRect.Height - titleSize.Height);
					break;
				}
				return titleFrame;
			}
			
			public override void DrawInteriorWithFrame (SD.RectangleF cellFrame, NSView inView)
			{
				var titleRect = this.TitleRectForBounds (cellFrame);
				this.AttributedStringValue.DrawString (titleRect);
			}
		}
		
		
		public class EtoLabel : NSTextField, IMacControl
		{
			public object Handler {
				get;
				set;
			}
			
			static IntPtr selAttributedStringValue = Selector.GetHandle ("attributedStringValue");
			static IntPtr selSetAttributedStringValue = Selector.GetHandle ("setAttributedStringValue:");
			
			public NSAttributedString AttributedStringValue {
				[Export ("attributedStringValue")]
				get {
					if (this.IsDirectBinding) {
						return new NSAttributedString (Messaging.IntPtr_objc_msgSend (base.Handle, EtoLabel.selAttributedStringValue));
					}
					return new NSAttributedString (Messaging.IntPtr_objc_msgSendSuper (base.SuperHandle, EtoLabel.selAttributedStringValue));
				}
				[Export ("setAttributedStringValue:")]
				set {
					if (value == null) {
						throw new ArgumentNullException ("value");
					}
					if (this.IsDirectBinding) {
						Messaging.void_objc_msgSend_IntPtr (base.Handle, EtoLabel.selSetAttributedStringValue, value.Handle);
					} else {
						Messaging.void_objc_msgSendSuper_IntPtr (base.SuperHandle, EtoLabel.selSetAttributedStringValue, value.Handle);
					}
				}
				
			}
		}
		
		public LabelHandler ()
		{
			Enabled = true;
			Control = new EtoLabel { Handler = this };
			Control.Cell = new MyTextFieldCell{ Handler = this };
			Control.DrawsBackground = false;
			Control.Bordered = false;
			Control.Bezeled = false;
			Control.Editable = false;
			Control.Selectable = false;
			Control.Alignment = NSTextAlignment.Left;
			is106 = Control.Cell.RespondsToSelector (new Selector ("setUsesSingleLineMode:"));
			if (is106)
				Control.Cell.UsesSingleLineMode = false;
			Control.Cell.LineBreakMode = NSLineBreakMode.ByWordWrapping;
		}
		
		public Color TextColor {
			get {
				return Generator.Convert (Control.TextColor);
			}
			set {
				Control.TextColor = Generator.ConvertNS (value);
			}
		}
		
		public WrapMode Wrap {
			get {
				if (Control.Cell.UsesSingleLineMode)
					return WrapMode.None;
				else if (Control.Cell.LineBreakMode == NSLineBreakMode.ByWordWrapping)
					return WrapMode.Word;
				else //if (Control.Cell.LineBreakMode == NSLineBreakMode.ByWordWrapping)
					return WrapMode.Character;
			}
			set {
				switch (value) {
				case WrapMode.None:
					if (is106)
						Control.Cell.UsesSingleLineMode = true;
					Control.Cell.LineBreakMode = NSLineBreakMode.Clipping;
					break;
				case WrapMode.Word:
					if (is106)
						Control.Cell.UsesSingleLineMode = false;
					Control.Cell.LineBreakMode = NSLineBreakMode.ByWordWrapping;
					break;
				case WrapMode.Character:
					if (is106)
						Control.Cell.UsesSingleLineMode = false;
					Control.Cell.LineBreakMode = NSLineBreakMode.CharWrapping;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}

		public override bool Enabled { get; set; }
		
		public string Text {
			get {
				return Control.StringValue;
			}
			set {
				var oldSize = GetPreferredSize ();
				
				var match = Regex.Match (value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
				if (match.Success) {
					var str = new NSMutableAttributedString (value.Remove(match.Index, match.Length).Replace ("&&", "&"));
					
					var matches = Regex.Matches (value, @"[&][&]");
					var prefixCount = matches.Cast<Match>().Count (r => r.Index < match.Index);
					
					// copy existing attributes
					NSRange range;
					var attributes = new NSMutableDictionary(Control.AttributedStringValue.GetAttributes (0, out range));
					if (attributes.ContainsKey(CTStringAttributeKey.UnderlineStyle))
						attributes.Remove (CTStringAttributeKey.UnderlineStyle);
					str.AddAttributes (attributes, new NSRange(0, str.Length));
					
					str.AddAttribute (CTStringAttributeKey.UnderlineStyle, new NSNumber ((int)CTUnderlineStyle.Single), new NSRange (match.Index - prefixCount, 1));
					Control.AttributedStringValue = str;
				} else if (!string.IsNullOrEmpty(value))
					Control.StringValue = value.Replace ("&&", "&");
				else
					Control.StringValue = string.Empty;
				LayoutIfNeeded (oldSize);
			}
		}
		
		public HorizontalAlign HorizontalAlign {
			get {
				switch (Control.Alignment) {
				case NSTextAlignment.Center:
					return HorizontalAlign.Center;
				case NSTextAlignment.Right:
					return HorizontalAlign.Right;
				default:
				case NSTextAlignment.Left:
					return HorizontalAlign.Left;
				}
			}
			set {
				switch (value) {
				case HorizontalAlign.Center:
					Control.Alignment = NSTextAlignment.Center;
					break;
				case HorizontalAlign.Right:
					Control.Alignment = NSTextAlignment.Right;
					break;
				case HorizontalAlign.Left:
					Control.Alignment = NSTextAlignment.Left;
					break;
				}
			}
		}
		
		public Eto.Drawing.Font Font {
			get {
				return font;
			}
			set {
				font = value;
				if (font != null)
					Control.Font = ((FontHandler)font.Handler).Control;
				else
					Control.Font = NSFont.LabelFontOfSize (NSFont.LabelFontSize);
			}
		}
		
		public VerticalAlign VerticalAlign { get; set; }

	}
}
