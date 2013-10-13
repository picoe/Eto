using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.ComponentModel;

#if XAML
using System.Windows.Markup;
#endif
namespace Eto.Forms
{
	[ContentProperty("Items")]
	public class DynamicRow
	{
		public DynamicTable Table { get; internal set; }

		public List<DynamicItem> Items { get; private set; }

		public DynamicRow()
		{
			Items = new List<DynamicItem>();
		}

		public DynamicRow(IEnumerable<DynamicItem> items)
		{
			Items = new List<DynamicItem>(items);
		}

		public DynamicRow(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			Items = new List<DynamicItem>();
			Add(controls, xscale, yscale);
		}

		public void Add(params Control[] controls)
		{
			Add((IEnumerable<Control>)controls);
		}

		public void Add(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			if (controls == null)
				return;
			var items = controls.Select(r => new DynamicControl { Control = r, YScale = yscale, XScale = xscale ?? (r != null ? null : (bool?)true) });
			Items.AddRange(items.OfType<DynamicItem>());
		}
	}

	[ContentProperty("Rows")]
	public class DynamicTable : DynamicItem, ISupportInitialize
	{
		bool visible = true;

		public List<DynamicRow> Rows { get; private set; }

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
			Rows = new List<DynamicRow>();
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
