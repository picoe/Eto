using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	[ContentProperty("Items"), TypeConverter(typeof(DynamicRowConverter))]
	public class DynamicRow
	{
		Collection<DynamicItem> items;

		public DynamicTable Table { get; internal set; }

		public Collection<DynamicItem> Items
		{ 
			get { return items ?? (items = new Collection<DynamicItem>()); }
			set { items = value; }
		}

		public DynamicRow()
		{
		}

		public DynamicRow(params DynamicItem[] items)
			: this((IEnumerable<DynamicItem>)items)
		{
		}

		public DynamicRow(IEnumerable<DynamicItem> items)
		{
			this.items = new Collection<DynamicItem>(items.ToList());
		}

		public DynamicRow(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			Add(controls, xscale, yscale);
		}

		public void Add(params Control[] controls)
		{
			Add(controls.AsEnumerable());
		}

		public void Add(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			if (controls == null)
				return;
			var controlItems = controls.Select(r => new DynamicControl { Control = r, YScale = yscale, XScale = xscale ?? (r != null ? null : (bool?)true) });
			foreach (var control in controlItems)
			{
				Items.Add(control);
			}
		}

		public static implicit operator DynamicRow(Control control)
		{
			var dynamicRow = new DynamicRow();
			dynamicRow.Items.Add(new DynamicControl { Control = control });
			return dynamicRow;
		}
	}

	[ContentProperty("Rows")]
	public class DynamicTable : DynamicItem, ISupportInitialize
	{
		Collection<DynamicRow> rows;
		bool visible = true;

		public Collection<DynamicRow> Rows
		{ 
			get { return rows ?? (rows = new Collection<DynamicRow>()); }
			set { rows = value; }
		}

		public TableLayout Table { get; private set; }

		public DynamicTable Parent { get; set; }

		public Padding? Padding { get; set; }

		public Size? Spacing { get; set; }

		public bool Visible
		{
			get { return Table != null ? Table.Visible : visible; }
			set
			{
				if (Table != null)
					Table.Visible = value;
				else
					visible = value;
			}
		}

		internal DynamicRow CurrentRow { get; set; }

		public DynamicTable()
		{
		}

		public DynamicTable(params DynamicRow[] rows)
			: this((IEnumerable<DynamicRow>)rows)
		{
		}

		public DynamicTable(IEnumerable<DynamicRow> rows)
		{
			Rows = new Collection<DynamicRow>(rows.ToList());
		}

		public void Add(DynamicItem item)
		{
			if (CurrentRow != null)
				CurrentRow.Items.Add(item);
			else
				AddRow(item);
		}

		public void AddRow(DynamicItem item)
		{
			var row = new DynamicRow();
			row.Table = this;
			row.Items.Add(item);
			Rows.Add(row);
		}

		public void AddRow(DynamicRow row)
		{
			row.Table = this;
			Rows.Add(row);
		}

		public override Control Create(DynamicLayout layout)
		{
			if (Rows.Count == 0)
				return null;
			int cols = Rows.Where(r => r != null).Max(r => r.Items.Count);

			Table = new TableLayout(cols, Rows.Count);
			var tableLayout = Table;
			var padding = Padding ?? layout.DefaultPadding;
			if (padding != null)
				tableLayout.Padding = padding.Value;

			var spacing = Spacing ?? layout.DefaultSpacing;
			if (spacing != null)
				tableLayout.Spacing = spacing.Value;

			var scalingRow = new DynamicRow();
			scalingRow.Items.Add(new DynamicControl { YScale = true });
			for (int cy = 0; cy < Rows.Count; cy++)
			{
				var row = Rows[cy] ?? scalingRow;
				for (int cx = 0; cx < row.Items.Count; cx++)
				{
					var item = row.Items[cx] ?? new DynamicControl { XScale = true };
					item.Create(layout, tableLayout, cx, cy);
				}
			}
			return Table;
		}

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
			foreach (var row in Rows)
			{
				row.Table = this;
			}
		}
	}
}
