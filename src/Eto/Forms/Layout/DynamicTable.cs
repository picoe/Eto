using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using sc = System.ComponentModel;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a row for a <see cref="DynamicTable"/>
	/// </summary>
	[sc.TypeConverter(typeof(DynamicRowConverter))]
	public class DynamicRow : Collection<DynamicItem>
	{
		/// <summary>
		/// Gets the table this row is contained in
		/// </summary>
		/// <value>The table.</value>
		public DynamicTable Table { get; internal set; }

		/// <summary>
		/// Gets or sets the items on this row.
		/// </summary>
		/// <value>The items contained in this row</value>
		[Obsolete("Since 2.2: The DynamicRow is now its own collection, so add directly")]
		public new Collection<DynamicItem> Items
		{ 
			get { return this; }
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
			: base(items.ToList())
		{
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
				base.Add(item);
		}

		/// <summary>
		/// Add the specified items to the row
		/// </summary>
		/// <param name="items">Items to add</param>
		public void Add(IEnumerable<DynamicItem> items)
		{
			foreach (var item in items)
				base.Add(item);
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
				base.Add(control);
			}
		}

		/// <summary>
		/// Implicitly converts a control to a row to easily define layouts
		/// </summary>
		/// <param name="control">Control to convert from</param>
		public static implicit operator DynamicRow(Control control)
		{
			var dynamicRow = new DynamicRow();
			dynamicRow.Add(new DynamicControl { Control = control });
			return dynamicRow;
		}

		internal void SetParent(DynamicTable table)
		{
			Table = table;
			foreach (var item in this)
			{
				if (item != null)
					item.SetParent(table);
			}
		}

		internal void SetLayout(DynamicLayout layout)
		{
			foreach (var item in this)
			{
				if (item != null)
					item.SetLayout(layout);
			}
		}

		/// <summary>
		/// Handles when an item is inserted into the collection
		/// </summary>
		/// <param name="index">Index for the inserted item</param>
		/// <param name="item">Item to insert</param>
		protected override void InsertItem(int index, DynamicItem item)
		{
			base.InsertItem(index, item);
			if (item != null)
				item.SetParent(Table);
		}

		/// <summary>
		/// Handles when an item is removed from the collection
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			if (item != null)
				item.SetParent(null);
		}

		/// <summary>
		/// Handles when the collection is cleared.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (var item in this)
			{
				if (item != null)
					item.SetParent(null);
			}
			base.ClearItems();
		}

		/// <summary>
		/// Handles when an item is changed
		/// </summary>
		/// <param name="index">Index of the item to change.</param>
		/// <param name="item">Item to change the item at the specified index to.</param>
		protected override void SetItem(int index, DynamicItem item)
		{
			var old = this[index];
			if (old != null)
				old.SetParent(null);
			base.SetItem(index, item);
			if (item != null)
				item.SetParent(Table);
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
		internal DynamicLayout layout;
		bool visible = true;
		readonly DynamicRowCollection rows;

		class DynamicRowCollection : Collection<DynamicRow>
		{
			readonly DynamicTable parent;

			public DynamicRowCollection(DynamicTable parent)
			{
				this.parent = parent;
			}

			public DynamicRowCollection(DynamicTable parent, IList<DynamicRow> rows)
				: base(rows)
			{
				this.parent = parent;
				foreach (var item in this)
				{
					if (item != null)
						item.SetParent(parent);
				}
			}

			protected override void InsertItem(int index, DynamicRow item)
			{
				base.InsertItem(index, item);
				if (item != null)
					item.SetParent(parent);
			}

			protected override void ClearItems()
			{
				foreach (var item in this)
				{
					if (item != null)
						item.SetParent(null);
				}
				base.ClearItems();
			}

			protected override void RemoveItem(int index)
			{
				var item = this[index];
				base.RemoveItem(index);
				if (item != null)
					item.SetParent(null);
			}

			protected override void SetItem(int index, DynamicRow item)
			{
				var old = this[index];
				if (old != null)
					old.SetParent(null);
				base.SetItem(index, item);
				if (item != null)
					item.SetParent(parent);
			}
		}

		/// <summary>
		/// Gets or sets the collection of rows in the table
		/// </summary>
		/// <value>The rows in the table.</value>
		public Collection<DynamicRow> Rows { get { return rows; } }

		/// <summary>
		/// Gets the table layout this item represents
		/// </summary>
		/// <value>The table.</value>
		public TableLayout Table { get; private set; }

		/// <summary>
		/// Gets or sets the parent table
		/// </summary>
		/// <value>The parent.</value>
		public DynamicTable Parent { get; private set; }

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
			rows = new DynamicRowCollection(this);
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
			this.rows = new DynamicRowCollection(this, rows.ToList());
		}

		/// <summary>
		/// Add the specified item to the current row
		/// </summary>
		/// <param name="item">Item to add</param>
		public void Add(DynamicItem item)
		{
			if (CurrentRow != null)
				CurrentRow.Add(item);
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
			row.Add(item);
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
			int cols = Rows.Where(r => r != null).Max(r => r.Count);

			var table = Table = new TableLayout(cols, Rows.Count);
			table.IsVisualControl = true;
			var padding = Padding ?? layout.DefaultPadding;
			if (padding != null)
				table.Padding = padding.Value;

			var spacing = Spacing ?? layout.DefaultSpacing;
			if (spacing != null)
				table.Spacing = spacing.Value;

			var scalingRow = new DynamicRow();
			scalingRow.Add(new DynamicControl { YScale = true });
			for (int cy = 0; cy < Rows.Count; cy++)
			{
				var row = Rows[cy] ?? scalingRow;
				for (int cx = 0; cx < row.Count; cx++)
				{
					var item = row[cx] ?? new DynamicControl { XScale = true };
					item.Create(layout, table, cx, cy);
				}
			}
			return table;
		}

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
		}

		internal override IEnumerable<Control> Controls
		{
			get
			{
				foreach (var row in Rows)
				{
					if (row == null)
						continue;
						
					foreach (var item in row)
					{
						if (item == null)
							continue;
						
						foreach (var control in item.Controls)
							yield return control;
					}
				}
			}
		}

		internal override void SetParent(DynamicTable table)
		{
			Parent = table;
			SetLayout(table != null ? table.layout : null);
		}

		internal override void SetLayout(DynamicLayout layout)
		{
			foreach (var row in rows)
			{
				if (row != null)
					row.SetLayout(layout);
			}
			this.layout = layout;
		}
	}

	/// <summary>
	/// Used to easily insert a <see cref="GroupBox"/> into a dynamic layout
	/// </summary>
	public class DynamicGroup : DynamicTable
	{
		string _title;

		/// <summary>
		/// Gets or sets the title of the group box.
		/// </summary>
		/// <value>The title of the groupbox.</value>
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				if (GroupBox != null)
					GroupBox.Text = value;
			}
		}

		/// <summary>
		/// Gets the group box instance when the layout has been generated.
		/// </summary>
		/// <value>The group box instance.</value>
		public GroupBox GroupBox { get; private set; }

		/// <summary>
		/// Creates the group box layout.
		/// </summary>
		/// <returns>The control created for this item.</returns>
		/// <param name="layout">Layout we are creating this item for.</param>
		public override Control Create(DynamicLayout layout)
		{
			return GroupBox = new GroupBox
			{
				Text = Title,
				Content = base.Create(layout)
			};
		}
	}

	/// <summary>
	/// Used to easily insert a <see cref="Scrollable"/> into a dynamic layout
	/// </summary>
	public class DynamicScrollable : DynamicTable
	{
		BorderType _border;

		/// <summary>
		/// Gets or sets the border for the contained scrollable
		/// </summary>
		public BorderType Border
		{
			get => _border;
			set
			{
				_border = value;
				if (Scrollable != null)
					Scrollable.Border = _border;
			}
		}

		/// <summary>
		/// Gets the Scrollable instance when the layout has been generated.
		/// </summary>
		/// <value>The Scrollable instance.</value>
		public Scrollable Scrollable { get; private set; }

		/// <summary>
		/// Creates the group box layout.
		/// </summary>
		/// <returns>The control created for this item.</returns>
		/// <param name="layout">Layout we are creating this item for.</param>
		public override Control Create(DynamicLayout layout)
		{
			return Scrollable = new Scrollable
			{
				Border = _border,
				Content = base.Create(layout)
			};
		}
	}

}
