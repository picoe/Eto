using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class CheckBoxCellHandler : SingleCellHandler<Gtk.CellRendererToggle, CheckBoxCell>, ICheckBoxCell
	{
		public CheckBoxCellHandler ()
		{
			Control = new Gtk.CellRendererToggle ();
			this.Control.Toggled += delegate(object o, Gtk.ToggledArgs args) {
				SetValue (args.Path, !Control.Active);
			};
		}
		
		protected override void BindCell (ref int dataIndex)
		{
			Column.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.AddAttribute (Control, "active", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Activatable = editable;
		}

		public override void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}
		
		public override GLib.Value GetValue (object item, int column)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
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

