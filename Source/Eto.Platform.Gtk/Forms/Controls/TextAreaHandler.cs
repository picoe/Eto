using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TextAreaHandler : GtkControl<Gtk.ScrolledWindow, TextArea>, ITextArea
	{
		Gtk.TextView textView;
		
		public TextAreaHandler ()
		{
			Control = new Gtk.ScrolledWindow ();
			Control.ShadowType = Gtk.ShadowType.In;
			textView = new Gtk.TextView ();
			Control.Add (textView);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				textView.Buffer.Changed += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override string Text {
			get { return textView.Buffer.Text; }
			set { textView.Buffer.Text = value; }
		}
		
		public bool ReadOnly {
			get { return !textView.Editable; }
			set { textView.Editable = !value; }
		}
		
		public bool Wrap {
			get { return textView.WrapMode != Gtk.WrapMode.None; }
			set { textView.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var end = textView.Buffer.EndIter;
			textView.Buffer.Insert (ref end, text);
			if (scrollToCursor) {
				var mark = textView.Buffer.CreateMark (null, end, false);
				textView.ScrollToMark (mark, 0, false, 0, 0);
			}
		}
		
		
	}
}
