using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class TextBoxCellHandler : SingleCellHandler<Gtk.CellRendererText, TextBoxCell>, ITextBoxCell
	{
		public TextBoxCellHandler ()
		{
			Control = new Gtk.CellRendererText ();
			this.Control.Edited += delegate(object o, Gtk.EditedArgs args) {
				SetValue (args.Path, args.NewText);
			};
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.AddAttribute (Control, "text", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Editable = editable;
		}
		
		public override void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value as string);
			}
		}
		
		public override GLib.Value GetValue (object item, int column)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
				if (ret != null)
					return new GLib.Value (Convert.ToString (ret));
			}
			return new GLib.Value ((string)null);
		}
		
		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.EndCellEditEvent:
				Control.Edited += (sender, e) => {
					Source.EndCellEditing (new Gtk.TreePath(e.Path), this.ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (eventHandler);
				break;
			}
		}
	}
}

