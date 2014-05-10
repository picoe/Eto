using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a row for a <see cref="DynamicTable"/>
	/// </summary>
	[ContentProperty("Items"), TypeConverter(typeof(DynamicRowConverter))]
	public class DynamicRow
	{
		Collection<DynamicItem> items;

		/// <summary>
		/// Gets the table this row is contained in
		/// </summary>
		/// <value>The table.</value>
		public DynamicTable Table { get; internal set; }

		/// <summary>
		/// Gets or sets the items on this row.
		/// </summary>
		/// <value>The items contained in this row</value>
		public Collection<DynamicItem> Items
		{ 
			get { return items ?? (items = new Collection<DynamicItem>()); }
			set { items = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicRow"/> class.
		/// </summary>
		public DynamicRow()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicRow"/> class.
		/// </summary>
		/// <param name="items">Items to initialize the row</param>
		public DynamicRow(params DynamicItem[] items)
			: this((IEnumerable<DynamicItem>)items)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicRow"/> class.
		/// </summary>
		/// <param name="items">Items to initialize the row</param>
		public DynamicRow(IEnumerable<DynamicItem> items)
		{
			this.items = new Collection<DynamicItem>(items.ToList());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicRow"/> class.
		/// </summary>
		/// <param name="controls">Items to initialize the row</param>
		/// <param name="xscale">Xscale.</param>
		/// <param name="yscale">Yscale.</param>
		public DynamicRow(IEnumerable<Control> controls, bool? xscale = null, bool? yscale = null)
		{
			Add(controls, xscale, yscale);
		}

		/// <summary>
		/// Add the specified controls to the row
		/// </summary>
		/// <param name="controls">Controls to add</param>
		public void Add(params Control[] controls)
		{
			Add(controls.AsEnumerable());
		}

		/// <summary>
		/// Add the specified items to the row
		/// </summary>
		/// <param name="items">Items to add</param>
		public void Add(params DynamicItem[] items)
		{
			foreach (var item in items)
				Items.Add(item);
		}

		/// <summary>
		/// Add the specified items to the row
		/// </summary>
		/// <param name="items">Items to add</param>
		public void Add(IEnumerable<DynamicItem> items)
		{
			foreach (var item in items)
				Items.Add(item);
		}

		/// <summary>
		/// Add the controls to the row, with specified xscale and yscale.
		/// </summary>
		/// <param name="controls">Controls to add</param>
		/// <param name="xscale">Horizontal scale for each control</param>
		/// <param name="yscale">Vertical scale for each control</param>
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

		/// <summary>
		/// Implicitly converts a control to a row to easily define layouts
		/// </summary>
		/// <param name="control">Control to convert from</param>
		public static implicit operator DynamicRow(Control control)
		{
			var dynamicRow = new DynamicRow();
			dynamicRow.Items.Add(new DynamicControl { Control = control });
			return dynamicRow;
		}
	}

	/// <summary>
	/// Table item for the <see cref="DynamicLayout"/>
	/// </summary>
	/// <remarks>
	/// This represents a table, which in a dynamic layout is used to represent a vertical section.
	/// The maximum number of items in the <see cref="DynamicTable.Rows"/> determines the columns of the table.
	/// </remarks>
	[ContentProperty("Rows")]
	public class DynamicTable : DynamicItem, ISupportInitialize
	{
		Collection<DynamicRow> rows;
		bool visible = true;

		/// <summary>
		/// Gets or sets the collection of rows in the table
		/// </summary>
		/// <value>The rows in the table.</value>
		public Collection<DynamicRow> Rows
		{ 
			get { return rows ?? (rows = new Collection<DynamicRow>()); }
			set { rows = value; }
		}

		/// <summary>
		/// Gets the table layout this item represents
		/// </summary>
		/// <value>The table.</value>
		public TableLayout Table { get; private set; }

		/// <summary>
		/// Gets or sets the parent table
		/// </summary>
		/// <value>The parent.</value>
		public DynamicTable Parent { get; set; }

		/// <summary>
		/// Gets or sets the padding around the table cells
		/// </summary>
		/// <value>The padding.</value>
		public Padding? Padding { get; set; }

		/// <summary>
		/// Gets or sets the spacing between the table cells
		/// </summary>
		/// <value>The spacing.</value>
		public Size? Spacing { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.DynamicTable"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicTable"/> class.
		/// </summary>
		public DynamicTable()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicTable"/> class.
		/// </summary>
		/// <param name="rows">Rows.</param>
		public DynamicTable(params DynamicRow[] rows)
			: this((IEnumerable<DynamicRow>)rows)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicTable"/> class.
		/// </summary>
		/// <param name="rows">Rows.</param>
		public DynamicTable(IEnumerable<DynamicRow> rows)
		{
			Rows = new Collection<DynamicRow>(rows.ToList());
		}

		/// <summary>
		/// Add the specified item to the current row
		/// </summary>
		/// <param name="item">Item to add</param>
		public void Add(DynamicItem item)
		{
			if (CurrentRow != null)
				CurrentRow.Items.Add(item);
			else
				AddRow(item);
		}

		/// <summary>
		/// Adds the specified item to a new row
		/// </summary>
		/// <param name="item">Item to add to a new row</param>
		public void AddRow(DynamicItem item)
		{
			var row = new DynamicRow();
			row.Table = this;
			row.Items.Add(item);
			Rows.Add(row);
		}

		/// <summary>
		/// Adds the specified row to the table
		/// </summary>
		/// <param name="row">Row to add</param>
		public void AddRow(DynamicRow row)
		{
			row.Table = this;
			Rows.Add(row);
		}

		/// <summary>
		/// Creates the content for this item
		/// </summary>
		/// <param name="layout">Top level layout the item is being created for</param>
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
