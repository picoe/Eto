using System;
using System.Collections;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp
{
	public class TableLayoutHandler : GtkLayout<Gtk.Table, TableLayout>, ITableLayout
	{
		Gtk.Alignment align;
		Dictionary<int, Gtk.AttachOptions> columnOptions = new Dictionary<int, Gtk.AttachOptions> ();
		Dictionary<int, Gtk.AttachOptions> rowOptions = new Dictionary<int, Gtk.AttachOptions> ();
		Control[,] controls;
		
		public override object ContainerObject {
			get {
				return align;
			}
		}
		
		public override void Update ()
		{
			this.Control.ResizeChildren ();
		}
		
		public Size Spacing {
			get { return new Size ((int)Control.ColumnSpacing, (int)Control.RowSpacing); }
			set { 
				Control.ColumnSpacing = (uint)value.Width;
				Control.RowSpacing = (uint)value.Height;
				
			}
		}
		
		public Padding Padding {
			get {
				uint top, bottom, left, right;
				align.GetPadding (out top, out bottom, out left, out right);
				return new Padding ((int)left, (int)top, (int)right, (int)bottom);
			}
			set {
				align.SetPadding ((uint)value.Top, (uint)value.Bottom, (uint)value.Left, (uint)value.Right);
			}
		}

		public void Add (Control child, int x, int y)
		{
			Attach (child, x, y);
			if (child != null) {
				var widget = (Gtk.Widget)child.ControlObject;
				widget.ShowAll ();
			}
		}

		public void Move (Control child, int x, int y)
		{
			Attach (child, x, y);
		}

		private Gtk.AttachOptions GetColumnOptions (int column)
		{
			if (controls.GetLength (1) == 1)
				return Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
			else if (columnOptions.ContainsKey (column))
				return columnOptions [column];
			else if (columnOptions.Count == 0 && column == controls.GetLength (1) - 1)
				return Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
			else
				return Gtk.AttachOptions.Fill;
		}

		private Gtk.AttachOptions GetRowOptions (int row)
		{
			if (controls.GetLength (0) == 1)
				return Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
			else if (rowOptions.ContainsKey (row))
				return rowOptions [row];
			else if (rowOptions.Count == 0 && row == controls.GetLength (0) - 1)
				return Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
			else
				return Gtk.AttachOptions.Fill;
		}
		
		public void CreateControl (int cols, int rows)
		{
			align = new Gtk.Alignment (0, 0, 1.0F, 1.0F);
			Control = new Gtk.Table ((uint)rows, (uint)cols, false);
			controls = new Control[rows, cols];
			align.Add (Control);

			this.Spacing = TableLayout.DefaultSpacing;
			this.Padding = TableLayout.DefaultPadding;
		}

		public void SetColumnScale (int column, bool scale)
		{
			Gtk.AttachOptions xopts = (scale) ? Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill : Gtk.AttachOptions.Fill;
			columnOptions [column] = xopts;
			bool found = false;
			var x = column;
			for (int y=0; y<controls.GetLength(0); y++) {
				found |= Attach (controls [y, x], x, y);
			}
			if (scale && !found) {
				Attach (new Panel (Widget.Generator), x, 0);
			}
		}
		
		bool Attach (Control child, int x, int y)
		{
			var old = controls [y, x];
			if (old != null && old != child) {
				if (old != null)
					Control.Remove ((Gtk.Widget)old.ControlObject);
			}
			
			if (child != null) {
				controls [y, x] = child;
				var widget = (Gtk.Widget)child.ControlObject;
				if (widget.Parent is Gtk.Container)
					((Gtk.Container)widget.Parent).Remove (widget); 
				Control.Attach (widget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions (x), GetRowOptions (y), 0, 0);
				return true;
			} else
				controls [y, x] = null;
			return false;
		}
		
		public void Remove (Control child)
		{
			bool done = false;
			for (int y = 0; y<controls.GetLength(0); y++) {
				for (int x = 0; x<controls.GetLength(1); x++) {
					if (Object.ReferenceEquals (controls [y, x], child)) {
						controls [y, x] = null;
						done = true;
						break;
					}
				}
				if (done)
					break;
			}
			Control.Remove ((Gtk.Widget)child.ControlObject);
		}
		
		public void SetRowScale (int row, bool scale)
		{
			Gtk.AttachOptions yopts = (scale) ? Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill : Gtk.AttachOptions.Fill;
			rowOptions [row] = yopts;
			bool found = false;
			var y = row;
			for (int x=0; x<controls.GetLength(1); x++) {
				found |= Attach (controls [y, x], x, y);
			}
			if (scale && !found) {
				Attach (new Panel (Widget.Generator), 0, y);
			}
		}

	}
}
