using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Collections;

namespace Eto.Forms
{

	public enum TableUnit
	{
		Auto,
		Pixel,
		Star
	}

	public struct TableLength
	{
		readonly TableUnit type;
		readonly float value;

		public TableUnit Type { get { return type; } }

		public float Value { get { return value; } }

		public bool IsStar { get { return type == TableUnit.Star; } }

		public bool IsAuto { get { return type == TableUnit.Auto; } }

		public bool IsAbsolute { get { return type == TableUnit.Pixel; } }

		public static readonly TableLength Auto = new TableLength(0, TableUnit.Auto);

		public static TableLength Star(float weight = 1)
		{
			return new TableLength(weight, TableUnit.Star);
		}

		public TableLength(float value, TableUnit type)
		{
			this.type = type;
			this.value = value;
		}

		public TableLength(float value)
		{
			this.type = TableUnit.Pixel;
			this.value = value;
		}

		public static implicit operator TableLength(float pixel)
		{
			return new TableLength(pixel);
		}

		public override string ToString()
		{
			switch (type)
			{
				case TableUnit.Auto:
					return "Auto";
				case TableUnit.Pixel:
					return value.ToString();
				case TableUnit.Star:
					return Math.Abs(value - 1.0f) < float.Epsilon ? "*" : value + "*";
				default:
					throw new NotSupportedException();
			}
		}
	}

	public class TableColumn
	{
		internal float MeasureWidth;

		public float ActualWidth { get; internal set; }

		public TableLength Width { get; set; }

		public TableLength MaxWidth { get; set; }

		public TableLength MinWidth { get; set; }
	}

	/// <summary>
	/// Represents the contents of a row in a <see cref="TableLayout"/> 
	/// </summary>
	[ContentProperty("Cells")]
	[TypeConverter(typeof(TableRowConverter))]
	public class TableRow2 : Collection<Control>
	{
		internal TableLayout2 Layout { get; set; }
		TableLayout2.IHandler Handler { get { return Layout != null ? Layout.Handler as TableLayout2.IHandler : null; } }

		internal float MeasureHeight;

		public float ActualHeight { get; internal set; }

		public TableLength MinHeight { get; set; }

		public TableLength MaxHeight { get; set; }

		public TableLength Height { get; set; }

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
		[Obsolete("Set Height to TableLenth.Star() instead")]
		public bool ScaleHeight
		{
			get { return Height.IsStar; }
			set
			{
				Height = value ? TableLength.Star() : TableLength.Auto;
			}
		}

		/// <summary>
		/// Gets or sets the cells in this row.
		/// </summary>
		/// <value>The cells in the row.</value>
		public Collection<Control> Cells
		{ 
			get { return this; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class.
		/// </summary>
		public TableRow2()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class with the specified cells.
		/// </summary>
		/// <param name="cells">Cells to populate the row.</param>
		public TableRow2(params Control[] cells)
		{
			foreach (var cell in cells)
				Add(cell);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableRow"/> class with the specified cells.
		/// </summary>
		/// <param name="cells">Cells to populate the row.</param>
		public TableRow2(IEnumerable<Control> cells)
		{
			foreach (var cell in cells)
				Add(cell);
		}

		/// <summary>
		/// Implicitly converts a control to a TableRow
		/// </summary>
		/// <remarks>
		/// Used to make defining a table's contents easier by allowing you to pass a control as a table row
		/// </remarks>
		/// <param name="control">Control to convert.</param>
		public static implicit operator TableRow2(Control control)
		{
			return new TableRow2(control);
		}

		/// <summary>
		/// Implicitly converts an array of cells to a TableRow
		/// </summary>
		/// <param name="cells">Cells to convert.</param>
		public static implicit operator TableRow2(Control[] cells)
		{
			return new TableRow2(cells);
		}

		/// <summary>
		/// Converts a string to a TableRow with a label control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
		/// </remarks>
		/// <param name="labelText">Text to convert to a Label control.</param>
		public static implicit operator TableRow2(string labelText)
		{
			return new TableRow2(new Label { Text = labelText });
		}

		/// <summary>
		/// Implicitly converts a TableRow to a control
		/// </summary>
		/// <remarks>
		/// Used to make defining a table's contents easier by allowing you to pass a table row as a control.
		/// </remarks>
		/// <param name="row">Row to convert.</param>
		public static implicit operator Control(TableRow2 row)
		{
			return new TableLayout2 { Rows = { row } };
		}

		protected override void InsertItem(int index, Control item)
		{
			//if (item == null)
			//	item = new Panel { HorizontalAlignment = HorizontalAlignment.Stretch };
			base.InsertItem(index, item);
			if (Layout != null)
			{
				Layout.EnsureColumns(index + 1);
				if (item != null)
					Handler.Add(item);
			}
		}

		protected override void SetItem(int index, Control item)
		{
			var handler = Handler;
			if (handler != null && this[index] != null)
			{
				handler.Remove(this[index]);
			}
			//if (item == null)
			//	item = new Panel { HorizontalAlignment = HorizontalAlignment.Stretch };
			base.SetItem(index, item);
			if (handler != null && item != null)
			{
				handler.Add(item);
			}
		}
	}
}
