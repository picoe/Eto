using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TextAreaHandler : MacTextControl<NSTextView, TextArea>, ITextArea
	{
		public class EtoTextView : NSTextView, IMacControl
		{
			public object Handler {
				get; set;
			}
		}

		public NSScrollView Scroll { get; private set; }

		public override NSView ContainerControl
		{
			get { return Scroll; }
		}
		
		public TextAreaHandler ()
		{
			Control = new EtoTextView {
				Handler = this,
				AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
				HorizontallyResizable = true,
				VerticallyResizable = true,
				Editable = true,
				Selectable = true,
				AllowsUndo = true
			};

			Scroll = new NSScrollView {
				AutoresizesSubviews = true,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = Control
			};
		}
		
		#region ITextArea Members
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				Control.TextDidChange += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public bool ReadOnly {
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}
		
		public override bool Enabled {
			get { return Control.Selectable; }
			set {
				Control.Selectable = value;
				if (!value) {
					Control.TextColor = NSColor.DisabledControlText;
					Control.BackgroundColor = NSColor.ControlBackground;
				} else {
					Control.TextColor = NSColor.ControlText;
					Control.BackgroundColor = NSColor.TextBackground;
				}
			}
		}
		
		public string Text {
			get {
				return Control.Value;
			}
			set {
				Control.Value = value;
				this.Control.DisplayIfNeeded ();
			}
		}
		
		public bool Wrap {
			get {
				return Control.TextContainer.WidthTracksTextView;
			}
			set {
				if (value) {
					Control.TextContainer.WidthTracksTextView = true;
				} else {
					Control.TextContainer.WidthTracksTextView = false;
					Control.TextContainer.ContainerSize = new System.Drawing.SizeF (float.MaxValue, float.MaxValue);
				}
			}
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var range = new NSRange (this.Control.Value.Length, 0);
			this.Control.Replace (range, text);
			range = new NSRange (this.Control.Value.Length, 0);
			this.Control.SelectedRange = range;
			if (scrollToCursor)
				this.Control.ScrollRangeToVisible (range);
		}
		
		#endregion
	}
}
