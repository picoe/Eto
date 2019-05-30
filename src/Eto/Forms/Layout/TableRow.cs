using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using sc = System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Collections;

namespace Eto.Forms
{

	/// <summary>
	/// Represents the contents of a row in a <see cref="TableLayout"/> 
	/// </summary>
	[ContentProperty("Cells")]
	[sc.TypeConverter(typeof(TableRowConverter))]
	public class TableRow
	{
		Collection<TableCell> cells;

		/// <summary>
		/// Creates a scaled table row with the specified <paramref name="cells"/>
		/// </summary>
		/// <remarks>
		/// This is the same as creating a new TableRow and setting ScaleHeight = true.
		/// </remarks>
		/// <param name="cells">Cells for the table row</param>
		/// <returns>A new scaled TableRow with the specified cells</returns>
		public static TableRow Scaled(params TableCell[] cells)
		{
			return new TableRow(cells) { ScaleHeight = true };
		}

		/// <summary>
		/// Creates a scaled table row with the specified <paramref name="cells"/>
		/// </summary>
		/// <remarks>
		/// This is the same as creating a new TableRow and setting ScaleHeight = true.
		/// </remarks>
		/// <param name="cells">Cells for the table row</param>
		/// <returns>A new scaled TableRow with the specified cells</returns>
		public static TableRow Scaled(IEnumerable<TableCell> cells)
		{
			return new TableRow(cells) { ScaleHeight = true };
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TableCell"/> will scale its height
		/// </summary>
		/// <remarks>
		/// All controls in the same row of this cell will get the same scaling value.
		/// Scaling will make the row expand to fit the rest of the height of the container, minus the preferred
		/// height of any non-scaled rows.
		/// 
		/// If there are no rows with height scaling, the last row will automatically get scaled.
		/// 
		/// With scaling turned off, cells in the row will fit the preferred size of the tallest control.
		/// </remarks>
		/// <value><c>true</c> if scale height; otherwise, <c>false</c>.</value>
		public bool ScaleHeight { get; set; }

		/// <summary>
		/// Gets or sets the cells in this row.
		/// </summary>
		/// <value>The cells in the row.</value>
		public Collection<TableCell> Cells
		{ 
			get { return cells ?? (cells = new TableCellCollection()); }
			set { cells = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class.
		/// </summary>
		public TableRow()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class with the specified cells.
		/// </summary>
		/// <param name="cells">Cells to populate the row.</param>
		public TableRow(params TableCell[] cells)
		{
			Cells = new TableCellCollection(cells);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class with the specified cells.
		/// </summary>
		/// <param name="cells">Cells to populate the row.</param>
		public TableRow(IEnumerable<TableCell> cells)
		{
			Cells = new TableCellCollection(cells);
		}

		/// <summary>
		/// Implicitly converts a control to a TableRow
		/// </summary>
		/// <remarks>
		/// Used to make defining a table's contents easier by allowing you to pass a control as a table row
		/// </remarks>
		/// <param name="control">Control to convert.</param>
		public static implicit operator TableRow(Control control)
		{
			return new TableRow(control);
		}

		/// <summary>
		/// Implicitly converts an array of cells to a TableRow
		/// </summary>
		/// <param name="cells">Cells to convert.</param>
		public static implicit operator TableRow(TableCell[] cells)
		{
			return new TableRow(cells);
		}

		/// <summary>
		/// Converts a string to a TableRow with a label control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
		/// </remarks>
		/// <param name="labelText">Text to convert to a Label control.</param>
		public static implicit operator TableRow(string labelText)
		{
			return new TableRow(new Label { Text = labelText });
		}

		/// <summary>
		/// Converts an <see cref="Image"/> to a TableRow with a <see cref="ImageView"/> control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add images to your layout through code, without having to create <see cref="ImageView"/> instances manually.
		/// </remarks>
		/// <param name="image">Image to convert to a TableRow with an ImageView control.</param>
		public static implicit operator TableRow(Image image)
		{
			return new TableRow(new ImageView { Image = image });
		}

		/// <summary>
		/// Implicitly converts a TableRow to a control
		/// </summary>
		/// <remarks>
		/// Used to make defining a table's contents easier by allowing you to pass a table row as a control.
		/// </remarks>
		/// <param name="row">Row to convert.</param>
		public static implicit operator Control(TableRow row)
		{
			return new TableLayout(row);
		}

		/// <summary>
		/// Implicitly converts a TableRow to a cell
		/// </summary>
		/// <remarks>
		/// Used to make defining a table's contents easier by allowing you to pass a table row as a cell
		/// without having to create a table layout and cell manually.
		/// </remarks>
		/// <param name="row">Row to convert.</param>
		public static implicit operator TableCell(TableRow row)
		{
			return new TableCell(new TableLayout(row));
		}

		internal void SetLayout(TableLayout layout)
		{
			((TableCellCollection)Cells).SetLayout(layout);
		}
	}

	class TableRowCollection : Collection<TableRow>, IList
	{
		TableLayout layout;
		public TableRowCollection(TableLayout layout)
		{
			this.layout = layout;
		}

		public TableRowCollection(TableLayout layout, IEnumerable<TableRow> list)
			: base(list.Select(r => r ?? new TableRow { ScaleHeight = true }).ToList())
		{
			this.layout = layout;
			foreach (var item in this)
				item?.SetLayout(layout);
		}

		protected override void RemoveItem(int index)
		{
			var item = this[index];
			item?.SetLayout(null);
			base.RemoveItem(index);
		}

		protected override void ClearItems()
		{
			foreach (var item in this)
				item?.SetLayout(null);
			base.ClearItems();
		}

		protected override void InsertItem(int index, TableRow item)
		{
			if (item == null)
				item = new TableRow { ScaleHeight = true };
			item?.SetLayout(layout);
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, TableRow item)
		{
			var old = this[index];
			old?.SetLayout(null);
			if (item == null)
				item = new TableRow { ScaleHeight = true };
			item?.SetLayout(layout);
			base.SetItem(index, item);
		}

		int IList.Add(object value)
		{
			// allow adding a control directly from xaml
			var control = value as Control;
			if (control != null)
				Add((TableRow)control);
			else
				Add((TableRow)value);
			return Count - 1;
		}

	}
}
