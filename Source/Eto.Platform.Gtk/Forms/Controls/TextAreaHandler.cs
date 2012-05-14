using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TextAreaHandler : GtkControl<Gtk.TextView, TextArea>, ITextArea
	{
		Gtk.ScrolledWindow scroll;

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}
		
		public TextAreaHandler ()
		{
			scroll = new Gtk.ScrolledWindow ();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new Gtk.TextView ();
			scroll.Add (Control);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				Control.Buffer.Changed += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override string Text {
			get { return Control.Buffer.Text; }
			set { Control.Buffer.Text = value; }
		}
		
		public bool ReadOnly {
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}
		
		public bool Wrap {
			get { return Control.WrapMode != Gtk.WrapMode.None; }
			set { Control.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var end = Control.Buffer.EndIter;
			Control.Buffer.Insert (ref end, text);
			if (scrollToCursor) {
				var mark = Control.Buffer.CreateMark (null, end, false);
				Control.ScrollToMark (mark, 0, false, 0, 0);
			}
		}
		
		
	}
}
