using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class TextCellHandler : CellHandler<Gtk.CellRendererText, TextCell>, ITextCell
	{
		public TextCellHandler ()
		{
			Control = new Gtk.CellRendererText();
			this.Control.Edited += delegate(object o, Gtk.EditedArgs args) {
				SetValue(args.Path, args.NewText);
			};
		}
		
		protected override void BindCell ()
		{
			Column.ClearAttributes (Control);
			Column.AddAttribute (Control, "text", ColumnIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Editable = editable;
		}
		
		public override void GetNullValue (ref GLib.Value val)
		{
			val = new GLib.Value((string)null);
		}
	}
}

