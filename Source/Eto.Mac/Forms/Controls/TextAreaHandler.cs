using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using sd = System.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using nnint = System.Int32;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
using nnint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
using nnint = System.Int32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TextAreaHandler : MacView<NSTextView, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		int? lastCaretIndex;
		Range<int> lastSelection;

		public class EtoTextView : NSTextView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public NSScrollView Scroll { get; private set; }

		public override NSView ContainerControl
		{
			get { return Scroll; }
		}
		// Remove use of delegate when events work correctly in MonoMac
		public class EtoDelegate : NSTextViewDelegate
		{
			WeakReference handler;

			public TextAreaHandler Handler { get { return (TextAreaHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void TextDidChange(NSNotification notification)
			{
				Handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
			}

			public override void DidChangeSelection(NSNotification notification)
			{
				var selection = Handler.Selection;
				if (selection != Handler.lastSelection)
				{
					Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
					Handler.lastSelection = selection;
				}
				var caretIndex = Handler.CaretIndex;
				if (caretIndex != Handler.lastCaretIndex)
				{
					Handler.Callback.OnCaretIndexChanged(Handler.Widget, EventArgs.Empty);
					Handler.lastCaretIndex = caretIndex;
				}
			}
		}

		public TextAreaHandler()
		{
			Control = new EtoTextView
			{
				Handler = this,
				Delegate = new EtoDelegate { Handler = this },
				AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
				HorizontallyResizable = true,
				VerticallyResizable = true,
				Editable = true,
				Selectable = true,
				AllowsUndo = true,
				MinSize = CGSize.Empty,
				MaxSize = new CGSize(float.MaxValue, float.MaxValue)
			};
			Control.TextContainer.WidthTracksTextView = true;

			Scroll = new EtoScrollView
			{
				Handler = this,
				AutoresizesSubviews = true,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = Control
			};
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return TextArea.DefaultSize;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
				/*Control.TextDidChange += (sender, e) => {
					Widget.OnTextChanged (EventArgs.Empty);
				};*/
					break;
				case TextArea.SelectionChangedEvent:
				/*Control.DidChangeSelection += (sender, e) => {
					var selection = this.Selection;
					if (selection != lastSelection) {
						Widget.OnSelectionChanged (EventArgs.Empty);
						lastSelection = selection;
					}
				};*/
					break;
				case TextArea.CaretIndexChangedEvent:
				/*Control.DidChangeSelection += (sender, e) => {
					var caretIndex = Handler.CaretIndex;
					if (caretIndex != lastCaretIndex) {
						Handler.Widget.OnCaretIndexChanged (EventArgs.Empty);
						lastCaretIndex = caretIndex;
					}
				};*/
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public override bool Enabled
		{
			get { return Control.Selectable; }
			set
			{
				Control.Selectable = value;
				if (!value)
				{
					Control.TextColor = NSColor.DisabledControlText;
					Control.BackgroundColor = NSColor.ControlBackground;
				}
				else
				{
					Control.TextColor = TextColor.ToNSUI();
					Control.BackgroundColor = BackgroundColor.ToNSUI();
				}
			}
		}

		public string Text
		{
			get
			{
				return Control.Value;
			}
			set
			{
				Control.TextStorage.SetString(font.AttributedString(value ?? string.Empty));
				Control.DisplayIfNeeded();
			}
		}

		Color? textColor;
		public Color TextColor
		{
			get { return textColor ?? Colors.Black; }
			set
			{
				textColor = value;
				if (textColor != null)
					Control.TextColor = textColor.Value.ToNSUI();
			}
		}

		Color? backgroundColor;
		public override Color BackgroundColor
		{
			get { return backgroundColor ?? Colors.White; }
			set
			{
				backgroundColor = value;
				if (backgroundColor != null)
					Control.BackgroundColor = backgroundColor.Value.ToNSUI();
			}
		}

		//public void SetBackgroundColor()
		//{
		//	if (base.backgroundColor != null)
		//	{
		//		Control.BackgroundColor = base.backgroundColor.Value.ToNSUI();
		//	}
		//}

		Font font;

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
				font = value;
				Control.TextStorage.SetString(font.AttributedString(Control.Value));
				Control.TypingAttributes = font.Attributes();
				LayoutIfNeeded();
			}
		}

		public bool Wrap
		{
			get
			{
				return Control.TextContainer.WidthTracksTextView;
			}
			set
			{
				if (value)
				{
					Control.TextContainer.WidthTracksTextView = true;
				}
				else
				{
					Control.TextContainer.WidthTracksTextView = false;
					Control.TextContainer.ContainerSize = new CGSize(float.MaxValue, float.MaxValue);
				}
			}
		}

		public string SelectedText
		{
			get
			{
				var range = Control.SelectedRange;
				if (range.Location >= 0 && range.Length > 0)
					return Control.Value.Substring((int)range.Location, (int)range.Length);
				return null;
			}
			set
			{
				var range = Control.SelectedRange;
				Control.TextStorage.DeleteRange(range);
				if (value != null)
				{
					range.Length = (nnint)value.Length;
					Control.TextStorage.Insert(new NSAttributedString(value), (nnint)range.Location);
					Control.SelectedRange = range;
				}
			}
		}

		public Range<int> Selection
		{
			get { return Control.SelectedRange.ToEto(); }
			set { Control.SelectedRange = value.ToNS(); }
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public int CaretIndex
		{
			get { return (int)Control.SelectedRange.Location; }
			set { Control.SelectedRange = new NSRange(value, 0); }
		}

		public void Append(string text, bool scrollToCursor)
		{
			var range = new NSRange(Control.Value.Length, 0);
			Control.Replace(range, text);
			range = new NSRange(Control.Value.Length, 0);
			Control.SelectedRange = range;
			if (scrollToCursor)
				Control.ScrollRangeToVisible(range);
		}
	}
}
