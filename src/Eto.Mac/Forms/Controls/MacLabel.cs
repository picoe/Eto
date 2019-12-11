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
				var lineHeight = CellSizeForBounds(theRect).Height;
				offset = (nfloat)Math.Round((theRect.Height - lineHeight) / 2.0F);
			}
			else if (VerticalAlignment == VerticalAlignment.Bottom)
			{
				var lineHeight = CellSizeForBounds(theRect).Height;
				offset = (nfloat)Math.Round(theRect.Height - lineHeight);
			}
			offset = (nfloat)Math.Max(0, offset);
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

	static class MacLabel
	{
		public static readonly object InSizingKey = new object();

		public static readonly object FontKey = new object();

		public static readonly object TextColorKey = new object();
	}

	public abstract class MacLabel<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl: NSTextField
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		readonly NSMutableAttributedString str;
		readonly NSMutableParagraphStyle paragraphStyle;
		int underlineIndex;
		Size availableSizeCached;
		SizeF? naturalSizeInfinity;
		SizeF lastSize;
		bool isSizing;

		public override NSView ContainerControl => Control;

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (float.IsPositiveInfinity(availableSize.Width))
			{
				if (naturalSizeInfinity != null)
					return naturalSizeInfinity.Value;

				var width = UserPreferredSize.Width;
				if (width < 0) width = int.MaxValue;
				var size = Control.Cell.CellSizeForBounds(new CGRect(0, 0, width, int.MaxValue)).ToEto();
				naturalSizeInfinity = Size.Ceiling(size);
				return naturalSizeInfinity.Value;
			}

			if (Widget.Loaded && Wrap != WrapMode.None && UserPreferredSize.Width > 0)
			{
				/*if (!float.IsPositiveInfinity(availableSize.Width))
					availableSize.Width = Math.Max(Size.Width, availableSize.Width);
				else*/
				availableSize.Width = UserPreferredSize.Width;
				availableSize.Height = float.PositiveInfinity;
			}

			var availableSizeTruncated = availableSize.TruncateInfinity();
			if (NaturalSize == null || availableSizeCached != availableSizeTruncated)
			{
				var size = Control.Cell.CellSizeForBounds(new CGRect(CGPoint.Empty, availableSizeTruncated.ToNS())).ToEto();
				NaturalSize = Size.Ceiling(size);
				availableSizeCached = availableSizeTruncated;
			}

			return NaturalSize.Value;
		}

		public override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (isSizing)
				return;
			isSizing = true;
			var size = Size;
			if (Wrap != WrapMode.None && lastSize.Width != size.Width && !Control.IsHiddenOrHasHiddenAncestor)
			{
				// when wrapping we use the current size, if it changes we check if we need another layout pass
				// this is needed when resizing a form/label so it can wrap correctly as GetNaturalSize()
				// will use the old size first, and won't necessarily know the final size of the label.
				lastSize = size;
				InvalidateMeasure();
			}
			isSizing = false;
		}

		protected MacLabel()
		{
			paragraphStyle = new NSMutableParagraphStyle();
			str = new NSMutableAttributedString();

			underlineIndex = -1;
			paragraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(Eto.Forms.Control.SizeChangedEvent);
		}

		protected override TControl CreateControl()
		{
			return new EtoLabel() as TControl;
		}

		public Color TextColor
		{
			get { return Widget.Properties.Get<Color?>(MacLabel.TextColorKey) ?? SystemColors.ControlText; }
			set
			{
				Widget.Properties[MacLabel.TextColorKey] = value;
				SetAttributes();
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
				if (paragraphStyle.LineBreakMode == NSLineBreakMode.Clipping)
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
						paragraphStyle.LineBreakMode = NSLineBreakMode.Clipping;
						break;
					case WrapMode.Word:
						paragraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
						break;
					case WrapMode.Character:
						paragraphStyle.LineBreakMode = NSLineBreakMode.CharWrapping;
						break;
					default:
						throw new NotSupportedException();
				}
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public string Text
		{
			get { return str.Value; }
			set
			{
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
				InvalidateMeasure();
			}
		}

		public TextAlignment TextAlignment
		{
			get { return paragraphStyle.Alignment.ToEto(); }
			set
			{
				paragraphStyle.Alignment = value.ToNS();
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public virtual Font Font
		{
			get
			{
				return Widget.Properties.Create<Font>(MacLabel.FontKey, () => new Font(new FontHandler(Control.Font)));
			}
			set
			{
				if (Widget.Properties.Get<Font>(MacLabel.FontKey) != value)
				{
					Widget.Properties[MacLabel.FontKey] = value;
					SetAttributes();
					InvalidateMeasure();
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
					Widget.Properties.Get<Font>(MacLabel.FontKey).Apply(attr);
					// need a copy of the paragraph style otherwise they don't get applied correctly when changed
					attr.Add(NSStringAttributeKey.ParagraphStyle, (NSParagraphStyle)paragraphStyle.Copy());
					var col = CurrentColor;	
					if (col != null)
						attr.Add(NSStringAttributeKey.ForegroundColor, col);
					str.SetAttributes(attr, range);
					if (underlineIndex >= 0)
					{
						var num = (NSNumber)str.GetAttribute(NSStringAttributeKey.UnderlineStyle, underlineIndex, out range);
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
				var col = Widget.Properties.Get<Color?>(MacLabel.TextColorKey);
				if (col != null)
					return col.Value.ToNSUI();
				return null; 
			}
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			naturalSizeInfinity = null;
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
