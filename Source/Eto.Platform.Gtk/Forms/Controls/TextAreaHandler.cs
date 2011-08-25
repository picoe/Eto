using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TextAreaHandler : GtkControl<Gtk.ScrolledWindow, TextArea>, ITextArea
	{
		Gtk.TextView textView;
		
		public TextAreaHandler()
		{
			Control = new Gtk.ScrolledWindow();
			Control.ShadowType = Gtk.ShadowType.In;
			textView = new Gtk.TextView();
			Control.Add(textView);
			//control.SetBorderWindowSize(Gtk.TextWindowType.Widget, 1);
			//control.ResizeMode = Gtk.ResizeMode.;
			//control.WrapMode = Gtk.WrapMode.None;
			//scroll.Add(control);
		}
		
		public override string Text
		{
			get { return textView.Buffer.Text; }
			set { textView.Buffer.Text = value; }
		}
		
		public bool ReadOnly
		{
			get { return !textView.Editable; }
			set { textView.Editable = !value; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var end = textView.Buffer.EndIter;
			textView.Buffer.Insert (ref end, text);
			if (scrollToCursor) textView.ScrollToIter (end, 0, false, 0, 0);
		}
		
		
	}
}
