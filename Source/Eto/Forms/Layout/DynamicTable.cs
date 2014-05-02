using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;

#if XAML
using System.Windows.Markup;

#endif
namespace Eto.Forms
{
	[ContentProperty("Items"), TypeConverter(typeof(DynamicRowConverter))]
	public class DynamicRow
	{
		readonly List<DynamicItem> list;

		readonly Collection<DynamicItem> items;
		public DynamicTable Table { get; internal set; }

		public Collection<DynamicItem> Items { get { return items; } }

		public DynamicRow()
		{
			items = new Collection<DynamicItem>(list = new List<DynamicItem>());
		}

		public DynamicRow(IEnumerable<DynamicItem> items)
		{
			this.items = new Collection<DynamicItem>(list = new List<DynamicItem>(items));
		}

		public DynamicRow(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			items = new Collection<DynamicItem>(list = new List<DynamicItem>());
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
			list.AddRange(controlItems.OfType<DynamicItem>());
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

		readonly Collection<DynamicRow> rows;
		bool visible = true;

		public Collection<DynamicRow> Rows { get { return rows; } }

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

			rows = new Collection<DynamicRow>();
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

		public override Control Generate(DynamicLayout layout)
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
					item.Generate(layout, tableLayout, cx, cy);
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
