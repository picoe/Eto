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
using VerticalAlign = Eto.Forms.VerticalAlign;

namespace Eto.Platform.Mac.Forms.Controls
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
		}
		
		public LabelHandler ()
		{
			Enabled = true;
			Control = new EtoLabel { 
				Handler = this,
				Cell = new MyTextFieldCell{ Handler = this, StringValue = string.Empty },
				DrawsBackground = false,
				Bordered = false,
				Bezeled = false,
				Editable = false,
				Selectable = false,
				Alignment = NSTextAlignment.Left
			};
			is106 = Control.Cell.RespondsToSelector (new Selector ("setUsesSingleLineMode:"));
			if (is106)
				Control.Cell.UsesSingleLineMode = false;
			Control.Cell.LineBreakMode = NSLineBreakMode.ByWordWrapping;
		}
		
		public Color TextColor {
			get {
				return Control.TextColor.ToEto ();
			}
			set {
				Control.TextColor = value.ToNS ();
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
				var oldSize = GetPreferredSize (Size.MaxValue);
				if (string.IsNullOrEmpty (value)) {
					Control.StringValue = string.Empty;
				}
				else {
					var match = Regex.Match (value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
					if (match.Success) {
						var val = value.Remove(match.Index, match.Length).Replace ("&&", "&");
						var str = new NSMutableAttributedString (val);
						
						var matches = Regex.Matches (value, @"[&][&]");
						var prefixCount = matches.Cast<Match>().Count (r => r.Index < match.Index);
						
						// copy existing attributes
						NSRange range;
						NSMutableDictionary attributes;
						if (Control.AttributedStringValue.Length > 0)
							attributes = new NSMutableDictionary(Control.AttributedStringValue.GetAttributes (0, out range));
						else
							attributes = new NSMutableDictionary();

						if (attributes.ContainsKey(CTStringAttributeKey.UnderlineStyle))
							attributes.Remove (CTStringAttributeKey.UnderlineStyle);
						str.AddAttributes (attributes, new NSRange(0, str.Length));
						
						str.AddAttribute (CTStringAttributeKey.UnderlineStyle, new NSNumber ((int)CTUnderlineStyle.Single), new NSRange (match.Index - prefixCount, 1));
						Control.AttributedStringValue = str;
					} else
						Control.StringValue = value.Replace ("&&", "&");
				}
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
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Control.Font));
				return font;
			}
			set {
				var oldSize = GetPreferredSize (Size.MaxValue);
				font = value;
				if (font != null)
					Control.Font = ((FontHandler)font.Handler).Control;
				else
					Control.Font = NSFont.LabelFontOfSize (NSFont.LabelFontSize);
				LayoutIfNeeded (oldSize);
			}
		}
		
		public VerticalAlign VerticalAlign { get; set; }

	}
}
