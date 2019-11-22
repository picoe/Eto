using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
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
	public class TextAreaHandler : TextAreaHandler<TextArea, TextArea.ICallback>, TextArea.IHandler
	{
	}

	public interface ITextAreaHandler
	{
		int SuppressSelectionChanged { get; }

		TextArea.ICallback Callback { get; }

		TextArea Widget { get; }

		Range<int> Selection { get; }

		int CaretIndex { get; }

		Range<int> lastSelection { get; set; }
		int? lastCaretIndex { get; set; }

		void PerformLayout();
	}

	public class EtoTextAreaDelegate : NSTextViewDelegate
	{
		WeakReference handler;

		public ITextAreaHandler Handler { get { return (ITextAreaHandler)handler.Target; } set { handler = new WeakReference(value); } }

		public override void TextDidChange(NSNotification notification)
		{
			Handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
		}

		public override void DidChangeSelection(NSNotification notification)
		{
			var handler = Handler;
			var selection = handler.Selection;
			if (handler.SuppressSelectionChanged == 0 && selection != Handler.lastSelection)
			{
				handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
				handler.lastSelection = selection;
			}
			var caretIndex = handler.CaretIndex;
			if (caretIndex != handler.lastCaretIndex)
			{
				handler.Callback.OnCaretIndexChanged(Handler.Widget, EventArgs.Empty);
				handler.lastCaretIndex = caretIndex;
			}
		}
	}

	public class EtoTextView : NSTextView, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public object Handler
		{
			get { return WeakHandler.Target; }
			set { WeakHandler = new WeakReference(value); }
		}

		public override void ChangeColor(NSObject sender)
		{
			// ignore color changes
		}

		public EtoTextView(ITextAreaHandler handler)
		{
			Delegate = new EtoTextAreaDelegate { Handler = handler };
			AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
			HorizontallyResizable = true;
			VerticallyResizable = true;
			Editable = true;
			RichText = false;
			AllowsDocumentBackgroundColorChange = false;
			Selectable = true;
			AllowsUndo = true;
			MinSize = CGSize.Empty;
			MaxSize = new CGSize(float.MaxValue, float.MaxValue);
			TextContainer.WidthTracksTextView = true;
		}

		public override void Layout()
		{
			if (MacView.NewLayout)
				base.Layout();
			(Handler as ITextAreaHandler)?.PerformLayout();
			if (!MacView.NewLayout)
				base.Layout();
		}
	}

	public class TextAreaHandler<TControl, TCallback> : MacView<NSTextView, TControl, TCallback>, TextArea.IHandler, ITextAreaHandler
		where TControl : TextArea
		where TCallback : TextArea.ICallback
	{
		int suppressSelectionChanged;
		int? ITextAreaHandler.lastCaretIndex { get; set; }
		Range<int> ITextAreaHandler.lastSelection { get; set; }
		int ITextAreaHandler.SuppressSelectionChanged => suppressSelectionChanged;

		public override void OnKeyDown(KeyEventArgs e)
		{
			if (!AcceptsTab)
			{
				if (e.KeyData == Keys.Tab)
				{
					if (Control.Window != null)
						Control.Window.SelectNextKeyView(Control);
					return;
				}
				if (e.KeyData == (Keys.Tab | Keys.Shift))
				{
					if (Control.Window != null)
						Control.Window.SelectPreviousKeyView(Control);
					return;
				}
			}
			if (!AcceptsReturn && e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				return;
			}
			base.OnKeyDown(e);
		}

		public NSScrollView Scroll { get; private set; }

		public override NSView ContainerControl
		{
			get { return Scroll; }
		}

		protected override NSTextView CreateControl()
		{
			return new EtoTextView(this);
		}

		protected override void Initialize()
		{
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
			base.Initialize();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return new SizeF(100, 60);
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

		static readonly object ReadOnly_Key = new object();

		public bool ReadOnly
		{
			get { return Widget.Properties.Get<bool>(ReadOnly_Key); }
			set { Widget.Properties.Set(ReadOnly_Key, value, () => Control.Editable = !value); }
		}

		protected override bool ControlEnabled
		{
			get => Control.Selectable;
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
					Control.Editable = !ReadOnly; // this gets set to false when Selectable is set to false, so we need to re-enable it.
				}
			}
		}

		public virtual string Text
		{
			get
			{
				return Control.Value;
			}
			set
			{
				Control.Value = value ?? string.Empty;
				Control.NeedsDisplay = true;
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		static readonly object TextColor_Key = new object();

		public Color TextColor
		{
			get { return Widget.Properties.Get(TextColor_Key, () => NSColor.ControlText.ToEto()); }
			set
			{
				Widget.Properties.Set(TextColor_Key, value, () =>
				{
					Control.TextColor = Control.InsertionPointColor = value.ToNSUI();
				});
			}
		}

		static readonly object BackgroundColor_Key = new object();

		public override Color BackgroundColor
		{
			get { return Widget.Properties.Get<Color>(BackgroundColor_Key, () => NSColor.ControlBackground.ToEto()); }
			set
			{
				Widget.Properties.Set(BackgroundColor_Key, value, () =>
				{
					Control.BackgroundColor = value.ToNSUI();
				});
			}
		}

		static readonly object Font_Key = new object();

		public Font Font
		{
			get { return Widget.Properties.Create(Font_Key, () => new Font(new FontHandler(Control.Font))); }
			set
			{
				Widget.Properties.Set(Font_Key, value, () =>
				{
					Control.Font = value.ToNS() ?? NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					InvalidateMeasure();
				});
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
					Control.NeedsLayout = true;
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
				return string.Empty;
			}
			set
			{
				suppressSelectionChanged++;
				var newText = value ?? string.Empty;
				var range = Control.SelectedRange;
				Control.Replace(range, newText);
				range.Length = newText.Length;
				suppressSelectionChanged--;
				Control.SetSelectedRange(range);
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public Range<int> Selection
		{
			get { return Control.SelectedRange.ToEto(); }
			set { Control.SetSelectedRange(value.ToNS()); }
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public int CaretIndex
		{
			get { return (int)Control.SelectedRange.Location; }
			set { Control.SetSelectedRange(new NSRange(value, 0)); }
		}

		static readonly object AcceptsTab_Key = new object();

		public bool AcceptsTab
		{
			get { return Widget.Properties.Get<bool?>(AcceptsTab_Key) ?? true; }
			set
			{
				Widget.Properties[AcceptsTab_Key] = value;
				if (!value)
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
			}
		}

		static readonly object AcceptsReturn_Key = new object();

		public bool AcceptsReturn
		{
			get { return Widget.Properties.Get<bool?>(AcceptsReturn_Key) ?? true; }
			set
			{
				Widget.Properties[AcceptsReturn_Key] = value;
				if (!value)
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
			}
		}

		static readonly IntPtr selGetString = Selector.GetHandle("string");

		public void Append(string text, bool scrollToCursor)
		{
			// get NSString object so we don't have to marshal the entire string to get its length
			var stringValuePtr = Messaging.IntPtr_objc_msgSend(Control.Handle, selGetString);
			var str = Runtime.GetNSObject<NSString>(stringValuePtr);

			var range = new NSRange(str != null ? str.Length : 0, 0);
			Control.Replace(range, text);
			range.Location += text.Length;
			Control.SetSelectedRange(range);
			if (scrollToCursor)
				Control.ScrollRangeToVisible(range);
		}

		public TextAlignment TextAlignment
		{
			get { return Control.Alignment.ToEto(); }
			set { Control.Alignment = value.ToNS(); }
		}

		public bool SpellCheck
		{
			get { return Control.ContinuousSpellCheckingEnabled; }
			set { Control.ContinuousSpellCheckingEnabled = value; }
		}

		public bool SpellCheckIsSupported { get { return true; } }

		TextArea.ICallback ITextAreaHandler.Callback
		{
			get { return Callback; }
		}

		TextArea ITextAreaHandler.Widget
		{
			get { return Widget; }
		}

		public void PerformLayout()
		{
			if (Wrap)
			{
				// set width of content to the size of the control when wrapping
				Control.TextContainer.ContainerSize = new CGSize(Scroll.DocumentVisibleRect.Size.Width, float.MaxValue);
			}
		}

		public TextReplacements TextReplacements
		{
			get
			{
				var replacements = TextReplacements.None;
				if (Control.AutomaticTextReplacementEnabled)
					replacements |= TextReplacements.Text;
				if (Control.AutomaticQuoteSubstitutionEnabled)
					replacements |= TextReplacements.Quote;
				if (Control.AutomaticDashSubstitutionEnabled)
					replacements |= TextReplacements.Dash;
				if (Control.AutomaticSpellingCorrectionEnabled)
					replacements |= TextReplacements.Spelling;
				return replacements;
			}
			set
			{
				Control.AutomaticTextReplacementEnabled = value.HasFlag(TextReplacements.Text);
				Control.AutomaticQuoteSubstitutionEnabled = value.HasFlag(TextReplacements.Quote);
				Control.AutomaticDashSubstitutionEnabled = value.HasFlag(TextReplacements.Dash);
				Control.AutomaticSpellingCorrectionEnabled = value.HasFlag(TextReplacements.Spelling);
			}
		}

		public TextReplacements SupportedTextReplacements
		{
			get { return TextReplacements.Quote | TextReplacements.Text | TextReplacements.Dash | TextReplacements.Spelling; }
		}
	}
}