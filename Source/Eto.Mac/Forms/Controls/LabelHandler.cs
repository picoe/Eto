using System;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using Eto.Mac.Drawing;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using System.Text.RegularExpressions;
using System.Linq;

namespace Eto.Mac.Forms.Controls
{
	public class LabelHandler : MacView<NSTextField, Label, Label.ICallback>, Label.IHandler
	{
		static readonly bool supportsSingleLine;
		Font font;
		readonly NSMutableAttributedString str;
		readonly NSMutableParagraphStyle paragraphStyle;
		NSColor textColor;
		int underlineIndex;
		SizeF availableSizeCached;
		const NSStringDrawingOptions DrawingOptions = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

		static LabelHandler()
		{
			supportsSingleLine = ObjCExtensions.ClassInstancesRespondToSelector(Class.GetHandle("NSTextFieldCell"), Selector.GetHandle("setUsesSingleLineMode:"));
		}

		public override NSView ContainerControl { get { return Control; } }

		static readonly Selector selAlignmentRectInsets = new Selector("alignmentRectInsets");

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (string.IsNullOrEmpty(Text))
				return Size.Empty;
			if (NaturalSize == null || availableSizeCached != availableSize)
			{
				var insets = Control.RespondsToSelector(selAlignmentRectInsets) ? Control.AlignmentRectInsets.ToEtoSize() : new Size(4, 2);
				var size = Control.Cell.CellSizeForBounds(new RectangleF(availableSize).ToSD()).ToEto();

				NaturalSize = Size.Round(size + insets);
				availableSizeCached = availableSize;
			}

			return NaturalSize.Value;
		}

		public class MyTextFieldCell : NSTextFieldCell
		{
			public VerticalAlign VerticalAlign { get; set; }

			public override sd.RectangleF DrawingRectForBounds(sd.RectangleF theRect)
			{
				var rect = base.DrawingRectForBounds(theRect);
				var titleSize = CellSizeForBounds(theRect);

				switch (VerticalAlign)
				{
					case VerticalAlign.Middle:
						rect.Y = theRect.Y + (theRect.Height - titleSize.Height) / 2.0F;
						break;
					case VerticalAlign.Top:
						// do nothing!
						break;
					case VerticalAlign.Bottom:
						rect.Y = theRect.Y + (theRect.Height - titleSize.Height);
						break;
				}
				return rect;
			}
		}

		public class EtoLabel : NSTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public LabelHandler()
		{
			Enabled = true;
			paragraphStyle = new NSMutableParagraphStyle();
			str = new NSMutableAttributedString();
			textColor = NSColor.Text;
			underlineIndex = -1;
			Control = new EtoLabel
			{ 
				Handler = this,
				Cell = new MyTextFieldCell(),
				DrawsBackground = false,
				Bordered = false,
				Bezeled = false,
				Editable = false,
				Selectable = false,
				Alignment = NSTextAlignment.Left,
			};
			if (supportsSingleLine)
				Control.Cell.UsesSingleLineMode = false;
			paragraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
		}

		public Color TextColor
		{
			get { return textColor.ToEto(); }
			set
			{
				textColor = value.ToNSUI();
				SetAttributes();
			}
		}

		public WrapMode Wrap
		{
			get
			{
				if (supportsSingleLine && Control.Cell.UsesSingleLineMode)
					return WrapMode.None;
				if (paragraphStyle.LineBreakMode == NSLineBreakMode.ByWordWrapping)
					return WrapMode.Word;
				return WrapMode.Character;
			}
			set
			{
				switch (value)
				{
					case WrapMode.None:
						if (supportsSingleLine)
							Control.Cell.UsesSingleLineMode = true;
						paragraphStyle.LineBreakMode = NSLineBreakMode.Clipping;
						break;
					case WrapMode.Word:
						if (supportsSingleLine)
							Control.Cell.UsesSingleLineMode = false;
						paragraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
						break;
					case WrapMode.Character:
						if (supportsSingleLine)
							Control.Cell.UsesSingleLineMode = false;
						paragraphStyle.LineBreakMode = NSLineBreakMode.CharWrapping;
						break;
					default:
						throw new NotSupportedException();
				}
				SetAttributes();
			}
		}

		public override bool Enabled { get; set; }

		public string Text
		{
			get { return str.Value; }
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				if (string.IsNullOrEmpty(value))
				{
					str.SetString(new NSMutableAttributedString());
				}
				else
				{
					var match = Regex.Match(value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
					if (match.Success)
					{
						var val = value.Remove(match.Index, match.Length).Replace("&&", "&");

						var matches = Regex.Matches(value, @"[&][&]");
						var prefixCount = matches.Cast<Match>().Count(r => r.Index < match.Index);

						str.SetString(new NSAttributedString(val));
						underlineIndex = match.Index - prefixCount;
					}
					else
					{
						str.SetString(new NSAttributedString(value.Replace("&&", "&")));
						underlineIndex = -1;
					}
				}
				SetAttributes();
				LayoutIfNeeded(oldSize);
			}
		}

		public HorizontalAlign HorizontalAlign
		{
			get
			{
				switch (paragraphStyle.Alignment)
				{
					case NSTextAlignment.Center:
						return HorizontalAlign.Center;
					case NSTextAlignment.Right:
						return HorizontalAlign.Right;
					default:
						return HorizontalAlign.Left;
				}
			}
			set
			{
				switch (value)
				{
					case HorizontalAlign.Center:
						paragraphStyle.Alignment = NSTextAlignment.Center;
						break;
					case HorizontalAlign.Right:
						paragraphStyle.Alignment = NSTextAlignment.Right;
						break;
					case HorizontalAlign.Left:
						paragraphStyle.Alignment = NSTextAlignment.Left;
						break;
				}
				SetAttributes();
			}
		}

		public Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.Font));
				return font;
			}
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				font = value;
				SetAttributes();
				LayoutIfNeeded(oldSize);
			}
		}

		public VerticalAlign VerticalAlign
		{
			get { return ((MyTextFieldCell)Control.Cell).VerticalAlign; }
			set { ((MyTextFieldCell)Control.Cell).VerticalAlign = value; }
		}

		void SetAttributes(bool force = false)
		{
			if (Widget.Loaded || force)
			{
				if (str.Length > 0)
				{
					var range = new NSRange(0, str.Length);
					var attr = new NSMutableDictionary();
					font.Apply(attr);
					attr.Add(NSAttributedString.ParagraphStyleAttributeName, paragraphStyle);
					attr.Add(NSAttributedString.ForegroundColorAttributeName, textColor);
					str.SetAttributes(attr, range);
					if (underlineIndex >= 0)
					{
						var num = (NSNumber)str.GetAttribute(NSAttributedString.UnderlineStyleAttributeName, underlineIndex, out range);
						var newStyle = (num != null && (NSUnderlineStyle)num.IntValue == NSUnderlineStyle.Single) ? NSUnderlineStyle.Double : NSUnderlineStyle.Single;
						str.AddAttribute(NSAttributedString.UnderlineStyleAttributeName, new NSNumber((int)newStyle), new NSRange(underlineIndex, 1));
					}
				}
				Control.AttributedStringValue = str;
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetAttributes(true);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
