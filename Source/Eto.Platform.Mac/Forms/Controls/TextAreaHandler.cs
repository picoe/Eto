using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class TextAreaHandler : MacView<NSScrollView, TextArea>, ITextArea
	{
		NSTextView text;
		
		public TextAreaHandler ()
		{
			text = new NSTextView {
				AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
				HorizontallyResizable = true,
				VerticallyResizable = true,
				Editable = true,
				Selectable = true,
				AllowsUndo = true
			};

			Control = new NSScrollView {
				AutoresizesSubviews = true,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = text
			};
		}
		
		#region ITextArea Members
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				text.TextDidChange += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public bool ReadOnly {
			get { return !text.Editable; }
			set { text.Editable = !value; }
		}
		
		public override bool Enabled {
			get { return text.Selectable; }
			set {
				text.Selectable = value;
				if (!value) {
					text.TextColor = NSColor.DisabledControlText;
					Control.BackgroundColor = NSColor.ControlBackground;
				} else {
					text.TextColor = NSColor.ControlText;
					Control.BackgroundColor = NSColor.TextBackground;
				}
			}
		}
		
		public string Text {
			get {
				return text.Value;
			}
			set {
				text.Value = value;
				this.text.DisplayIfNeeded ();
			}
		}
		
		public bool Wrap {
			get {
				return text.TextContainer.WidthTracksTextView;
			}
			set {
				if (value) {
					text.TextContainer.WidthTracksTextView = true;
				} else {
					text.TextContainer.WidthTracksTextView = false;
					text.TextContainer.ContainerSize = new System.Drawing.SizeF (float.MaxValue, float.MaxValue);
				}
			}
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var range = new NSRange (this.text.Value.Length, 0);
			this.text.Replace (range, text);
			range = new NSRange (this.text.Value.Length, 0);
			this.text.SelectedRange = range;
			if (scrollToCursor)
				this.text.ScrollRangeToVisible (range);
		}
		
		#endregion
	}
}
