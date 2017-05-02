using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Text.RegularExpressions;
using System.Linq;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if XAMMAC
using nnint = System.Int32;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{

	public class EtoLabelFieldCell : NSTextFieldCell
	{
		public EtoLabelFieldCell()
		{
		}

		public EtoLabelFieldCell(IntPtr handle)
			: base(handle)
		{
		}

		/// <summary>
		/// Draws background color manually, using a layer or the base.BackgroundColor does not draw allow us to align the text properly.
		/// </summary>
		public NSColor BetterBackgroundColor { get; set; }

		[Export("verticalAlignment")]
		public VerticalAlignment VerticalAlignment { get; set; }

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var rect = base.DrawingRectForBounds(theRect);

			if (VerticalAlignment == VerticalAlignment.Top || VerticalAlignment == VerticalAlignment.Stretch)
				return rect;

			nfloat offset = 0;
			if (VerticalAlignment == VerticalAlignment.Center)
			{
				//var lineHeight = (nfloat)Font.LineHeight();
				var lineHeight = AttributedStringValue.BoundingRect(theRect.Size, NSStringDrawingOptions.UsesLineFragmentOrigin).Size.Height;
				//var lineHeight = CellSizeForBounds(theRect).Height;
				//lineHeight += Font.Descender;
				//var lineHeight = Math.Ceiling(Font.Ascender + Font.Leading + 1);
				//var lineHeight = Font.PointSize;
				offset = (nfloat)Math.Round((theRect.Height - lineHeight) / 2.0F);
			}
			else if (VerticalAlignment == VerticalAlignment.Bottom)
			{
				var lineHeight = CellSizeForBounds(theRect).Height;
				offset = (nfloat)Math.Round(theRect.Height - lineHeight);
			}
			rect.Y += offset;
			rect.Height -= offset;
			return rect;
		}

		public override void DrawWithFrame(CGRect cellFrame, NSView inView)
		{
			if (BetterBackgroundColor != null)
			{
				BetterBackgroundColor.SetFill();
				NSGraphics.RectFill(cellFrame);
			}
			base.DrawWithFrame(cellFrame, inView);
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

		public EtoLabel()
		{
			Cell = new EtoLabelFieldCell();
			DrawsBackground = false;
			Bordered = false;
			Bezeled = false;
			Editable = false;
			Selectable = false;
			Alignment = NSTextAlignment.Left;
		}
	}

	public abstract class MacLabel<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl: NSTextField
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		static readonly bool supportsSingleLine;
		readonly NSMutableAttributedString str;
		readonly NSMutableParagraphStyle paragraphStyle;
		int underlineIndex;
		SizeF availableSizeCached;
		const NSStringDrawingOptions DrawingOptions = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

		static MacLabel()
		{
			supportsSingleLine = ObjCExtensions.ClassInstancesRespondToSelector(Class.GetHandle("NSTextFieldCell"), Selector.GetHandle("setUsesSingleLineMode:"));
		}

		public override NSView ContainerControl { get { return Control; } }

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (NaturalSize == null || availableSizeCached != availableSize)
			{
				var size = Control.Cell.CellSizeForBounds(new CGRect(CGPoint.Empty, availableSize.ToNS())).ToEto();
				NaturalSize = Size.Ceiling(size);
				availableSizeCached = availableSize;
			}

			return NaturalSize.Value;
		}

		protected MacLabel()
		{
			Enabled = true;
			paragraphStyle = new NSMutableParagraphStyle();
			str = new NSMutableAttributedString();

			underlineIndex = -1;
			paragraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
		}

		protected override void Initialize()
		{
			if (supportsSingleLine)
				Control.Cell.UsesSingleLineMode = false;

			base.Initialize();
		}

		protected override TControl CreateControl()
		{
			return new EtoLabel() as TControl;
		}

		static readonly object TextColorKey = new object();

		public Color TextColor
		{
			get { return Widget.Properties.Get<Color?>(TextColorKey) ?? NSColor.Text.ToEto(); }
			set
			{
				if (value != TextColor)
				{
					Widget.Properties[TextColorKey] = value;
					SetAttributes();
				}
			}
		}

		protected override void SetBackgroundColor(Color? color)
		{
			var cell = Control.Cell as EtoLabelFieldCell;
			if (cell != null)
			{
				cell.BetterBackgroundColor = color?.ToNSUI();
				Control.SetNeedsDisplay();
			}
			else
				base.SetBackgroundColor(color);
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
				LayoutIfNeeded();
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

		public TextAlignment TextAlignment
		{
			get { return paragraphStyle.Alignment.ToEto(); }
			set
			{
				paragraphStyle.Alignment = value.ToNS();
				SetAttributes();
			}
		}

		static readonly object FontKey = new object();

		public virtual Font Font
		{
			get
			{
				return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control.Font)));
			}
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					var oldSize = GetPreferredSize(Size.MaxValue);
					Widget.Properties[FontKey] = value;
					SetAttributes();
					LayoutIfNeeded(oldSize);
				}
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return ((EtoLabelFieldCell)Control.Cell).VerticalAlignment; }
			set
			{
				((EtoLabelFieldCell)Control.Cell).VerticalAlignment = value;
				Control.SetNeedsDisplay();
			}
		}

		protected virtual void SetAttributes()
		{
			SetAttributes(false);
		}

		void SetAttributes(bool force)
		{
			if (Widget.Loaded || force)
			{
				if (str.Length > 0)
				{
					var range = new NSRange(0, (int)str.Length);
					var attr = new NSMutableDictionary();
					Widget.Properties.Get<Font>(FontKey).Apply(attr);
					attr.Add(NSStringAttributeKey.ParagraphStyle, paragraphStyle);
					var col = CurrentColor;	
					if (col != null)
						attr.Add(NSStringAttributeKey.ForegroundColor, col);
					str.SetAttributes(attr, range);
					if (underlineIndex >= 0)
					{
						var num = (NSNumber)str.GetAttribute(NSStringAttributeKey.UnderlineStyle, (nnint)underlineIndex, out range);
						var newStyle = (num != null && (NSUnderlineStyle)num.Int64Value == NSUnderlineStyle.Single) ? NSUnderlineStyle.Double : NSUnderlineStyle.Single;
						str.AddAttribute(NSStringAttributeKey.UnderlineStyle, new NSNumber((int)newStyle), new NSRange(underlineIndex, 1));
					}
				}
				Control.AttributedStringValue = str;
			}
		}

		protected virtual NSColor CurrentColor
		{
			get { 
				var col = Widget.Properties.Get<Color?>(TextColorKey);
				if (col != null)
					return col.Value.ToNSUI();
				return null; 
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
