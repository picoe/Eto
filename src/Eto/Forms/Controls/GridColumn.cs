using System;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Column collection for the <see cref="Grid"/>
	/// </summary>
	public class GridColumnCollection : ObservableCollection<GridColumn>
	{
	}

	/// <summary>
	/// Grid column definition for a <see cref="Grid"/>
	/// </summary>
	[Handler(typeof(GridColumn.IHandler))]
	[ContentProperty("DataCell")]
	public class GridColumn : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the text to display in the header of the column.
		/// </summary>
		/// <value>The header text.</value>
		public string HeaderText
		{
			get { return Handler.HeaderText; }
			set { Handler.HeaderText = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column is resizable by the user.
		/// </summary>
		/// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
		public bool Resizable
		{
			get { return Handler.Resizable; }
			set { Handler.Resizable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this column will auto size to the content of the grid.
		/// </summary>
		/// <remarks>
		/// This usually will only auto size based on the visible content to be as performant as possible.
		/// </remarks>
		/// <value><c>true</c> to auto size the column; otherwise, <c>false</c>.</value>
		public bool AutoSize
		{
			get { return Handler.AutoSize; }
			set { Handler.AutoSize = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can click on the header.
		/// </summary>
		/// <seealso cref="Grid.ColumnHeaderClick"/>
		/// <value><c>true</c> if the user can click the header; otherwise, <c>false</c>.</value>
		public bool Sortable
		{
			get { return Handler.Sortable; }
			set { Handler.Sortable = value; }
		}

		/// <summary>
		/// Gets or sets the initial width of the column.
		/// </summary>
		/// <value>The width of the column.</value>
		public int Width
		{
			get { return Handler.Width; }
			set { Handler.Width = value; }
		}

		/// <summary>
		/// Gets or sets the cell for the content of the column.
		/// </summary>
		/// <value>The column data cell.</value>
		public Cell DataCell
		{
			get { return Handler.DataCell; }
			set { Handler.DataCell = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can edit the contents of the cells, if the <see cref="DataCell"/> allows it.
		/// </summary>
		/// <value><c>true</c> if the data cell is editable; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get { return Handler.Editable; }
			set { Handler.Editable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this column is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible
		{
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this column should expand to fill the available space in the Grid
		/// </summary>
		/// <remarks>
		/// If more than one column has this set, they will share the space evenly.
		/// </remarks>
		/// <value><c>true</c> to expand the column; otherwise, <c>false</c></value>
		public bool Expand
		{
			get => Handler.Expand;
			set => Handler.Expand = value;
		}

		/// <summary>
		/// Gets or sets a value to specify the header text alignment for this column
		/// </summary>
		/// <value>TextAlignment for the column header</value>
		public TextAlignment HeaderTextAlignment
		{
			get => Handler.HeaderTextAlignment;
			set => Handler.HeaderTextAlignment = value;
		}

		/// <summary>
		/// Gets or sets the minimum width for the column
		/// </summary>
		/// <value>The column minimum width</value>
		public int MinWidth
		{
			get => Handler.MinWidth;
			set => Handler.MinWidth = value;
		}

		/// <summary>
		/// Gets or sets the maximum width for the column
		/// </summary>
		/// <value>The column maximum width</value>
		public int MaxWidth
		{
			get => Handler.MaxWidth;
			set => Handler.MaxWidth = value;
		}

		/// <summary>
		/// Handler interface for the <see cref="GridColumn"/>.
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets or sets the text to display in the header of the column.
			/// </summary>
			/// <value>The header text.</value>
			string HeaderText { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the column is resizable by the user.
			/// </summary>
			/// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
			bool Resizable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can click on the header.
			/// </summary>
			/// <seealso cref="Grid.ColumnHeaderClick"/>
			/// <value><c>true</c> if the user can click the header; otherwise, <c>false</c>.</value>
			bool Sortable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this column will auto size to the content of the grid.
			/// </summary>
			/// <remarks>
			/// This usually will only auto size based on the visible content to be as performant as possible.
			/// </remarks>
			/// <value><c>true</c> to auto size the column; otherwise, <c>false</c>.</value>
			bool AutoSize { get; set; }

			/// <summary>
			/// Gets or sets the initial width of the column.
			/// </summary>
			/// <value>The width of the column.</value>
			int Width { get; set; }

			/// <summary>
			/// Gets or sets the cell for the content of the column.
			/// </summary>
			/// <value>The column data cell.</value>
			Cell DataCell { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can edit the contents of the cells, if the <see cref="DataCell"/> allows it.
			/// </summary>
			/// <value><c>true</c> if the data cell is editable; otherwise, <c>false</c>.</value>
			bool Editable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this column is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this column should expand to fill the available space in the Grid
			/// </summary>
			/// <remarks>
			/// If more than one column has this set, they will share the space evenly.
			/// </remarks>
			/// <value><c>true</c> to expand the column; otherwise, <c>false</c></value>
			bool Expand { get; set; }

			/// <summary>
			/// Gets or sets a value to specify the header text alignment for this column
			/// </summary>
			/// <value>TextAlignment for the column header</value>
			TextAlignment HeaderTextAlignment { get; set; }

			/// <summary>
			/// Gets or sets the minimum width for the column
			/// </summary>
			/// <value>The column minimum width</value>
			int MinWidth { get; set; }

			/// <summary>
			/// Gets or sets the maximum width for the column
			/// </summary>
			/// <value>The column maximum width</value>
			int MaxWidth { get; set; }
		}
	}
}

