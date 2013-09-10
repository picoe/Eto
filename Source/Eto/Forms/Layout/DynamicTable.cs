using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		List<DynamicItem> items = new List<DynamicItem> ();

		public DynamicTable Table { get; internal set; }

		public List<DynamicItem> Items
		{
			get { return items; }
		}

		public DynamicRow ()
		{ }

		public DynamicRow (IEnumerable<DynamicItem> items)
		{
			this.items.AddRange (items);
		}

		public DynamicRow (IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			Add (controls, xscale, yscale);
		}
		
		public void Add (params Control[] controls)
		{
			Add ((IEnumerable<Control>)controls);
		}
			
		public void Add (IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			if (controls == null)
				return;
			var items = controls.Select (r => new DynamicControl { Control = r, YScale = yscale, XScale = r != null ? null : (bool?)true });
			Items.AddRange (items.OfType<DynamicItem>());
		}
	}

	[ContentProperty("Rows")]
	public class DynamicTable : DynamicItem, ISupportInitialize
	{
		bool visible = true;
		List<DynamicRow> rows = new List<DynamicRow> ();

		public List<DynamicRow> Rows
		{
			get { return rows; }
		}

		public TableLayout Table { get; private set; }

		public DynamicTable Parent { get; set; }

		public Padding? Padding { get; set; }

		public Size? Spacing { get; set; }

		public bool Visible
		{
			get { return Table != null ? Table.Visible : visible; }
			set {
				if (Table != null)
					Table.Visible = value;
				else
					visible = value;
			}
		}

		internal DynamicRow CurrentRow { get; set; }

		public void Add (DynamicItem item)
		{
			if (CurrentRow != null)
				CurrentRow.Items.Add (item);
			else
				AddRow (item);
		}

		public void AddRow (DynamicItem item)
		{
			var row = new DynamicRow ();
			row.Table = this;
			row.Items.Add (item);
			rows.Add (row);
		}

		public void AddRow (DynamicRow row)
		{
			row.Table = this;
			rows.Add (row);
		}

		public override Control Generate (DynamicLayout layout)
		{
			if (rows.Count == 0)
				return null;
			int cols = rows.Where (r => r != null).Max (r => r.Items.Count);

			this.Table = new TableLayout (cols, rows.Count);
			var tableLayout = this.Table;
			var padding = this.Padding ?? layout.DefaultPadding;
			if (padding != null)
				tableLayout.Padding = padding.Value;

			var spacing = this.Spacing ?? layout.DefaultSpacing;
			if (spacing != null)
				tableLayout.Spacing = spacing.Value;

			var scalingRow = new DynamicRow ();
			scalingRow.Items.Add (new DynamicControl{ YScale = true });
			for (int cy = 0; cy < rows.Count; cy++) {
				var row = rows[cy];
				if (row == null) row = scalingRow;
				for (int cx = 0; cx < row.Items.Count; cx++) {
					var item = row.Items[cx];
					if (item == null) item = new DynamicControl { XScale = true };
					item.Generate (layout, tableLayout, cx, cy);
				}
			}
			return Table;
		}

		void ISupportInitialize.BeginInit ()
		{
		}

		void ISupportInitialize.EndInit ()
		{
			foreach (var row in rows)
			{
				row.Table = this;
			}
		}
	}
}
