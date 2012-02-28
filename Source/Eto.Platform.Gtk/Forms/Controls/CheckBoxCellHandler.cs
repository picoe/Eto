using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<Gtk.CellRendererToggle, CheckBoxCell>, ICheckBoxCell
	{
		public CheckBoxCellHandler ()
		{
			Control = new Gtk.CellRendererToggle ();
			this.Control.Toggled += delegate(object o, Gtk.ToggledArgs args) {
				SetValue (args.Path, !Control.Active);
			};
		}
		
		protected override void BindCell ()
		{
			Column.ClearAttributes (Control);
			Column.AddAttribute (Control, "active", ColumnIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Activatable = editable;
		}
		
		public override GLib.Value GetValue (IGridItem item, int column)
		{
			if (item != null) {
				var ret = item.GetValue (column);
				if (ret != null)
					return new GLib.Value (ret);
			}
			return new GLib.Value ((bool)false);
		}

		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.EndCellEditEvent:
				Control.Toggled += (sender, e) => {
					Source.EndCellEditing (e.Path, this.ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (eventHandler);
				break;
			}
		}
	}
}

