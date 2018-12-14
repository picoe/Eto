using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Layout for controls in a table
	/// </summary>
	/// <remarks>
	/// This is similar to an html table, though each control will fill its entire cell.
	/// </remarks>
	[ContentProperty("Rows")]
	[Handler(typeof(TableLayout.IHandler))]
	public class TableLayout : Layout
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static object rowsKey = new object();
		Size dimensions;
		bool created;

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get { return Rows.SelectMany(r => r.Cells).Select(r => r.Control).Where(r => r != null); }
		}

		/// <summary>
		/// Gets the collection of rows in the table
		/// </summary>
		/// <value>The rows.</value>
		public Collection<TableRow> Rows
		{
			get { return Properties.Create(rowsKey, () => new TableRowCollection(this)); }
			private set { Properties[rowsKey] = value; }
		}

		/// <summary>
		/// Gets the dimensions of the table in cells.
		/// </summary>
		/// <value>The dimensions of the table.</value>
		public Size Dimensions
		{
			get { return dimensions; }
		}

		/// <summary>
		/// Creates a table layout with an auto sized control.
		/// </summary>
		/// <remarks>
		/// Since controls fill an entire cell, you can use this method to create a layout that will ensure that the
		/// specified <paramref name="control"/> gets its preferred size instead of stretching to fill the container.
		/// 
		/// By default, extra space will be added to the right and bottom, unless <paramref name="centered"/> is <c>true</c>,
		/// which will add equal space to the top/bottom, and left/right.
		/// </remarks>
		/// <returns>The table layout with the auto sized control.</returns>
		/// <param name="control">Control to auto size.</param>
		/// <param name="padding">Padding around the control</param>
		/// <param name="centered">If set to <c>true</c> center the control, otherwise control is upper left of the container.</param>
		public static TableLayout AutoSized(Control control, Padding? padding = null, bool centered = false)
		{
			if (centered)
			{
				var layout = new TableLayout(3, 3);
				layout.Padding = padding ?? Padding.Empty;
				layout.Spacing = Size.Empty;
				layout.SetColumnScale(0);
				layout.SetColumnScale(2);
				layout.SetRowScale(0);
				layout.SetRowScale(2);
				layout.Add(control, 1, 1);
				return layout;
			}
			else
			{
				var layout = new TableLayout(2, 2);
				layout.Padding = padding ?? Padding.Empty;
				layout.Spacing = Size.Empty;
				layout.Add(control, 0, 0);
				return layout;
			}
		}

		/// <summary>
		/// Creates a horizontal table layout with the specified cells.
		/// </summary>
		/// <remarks>
		/// Since table layouts are by default vertical by defining the rows and the cells for each row,
		/// it is verbose to create nested tables when you want a horizontal table.  E.g. <code>new TableLayout(new TableRow(...))</code>.
		/// 
		/// This method is used to easily create a single row table layout with a horizontal set of cells. E.g.
		/// <code>TableLayout.Horizontal(...)</code>
		/// </remarks>
		/// <param name="cells">Cells for the row</param>
		/// <returns>A new single row table layout with the specified cells</returns>
		public static TableLayout Horizontal(params TableCell[] cells)
		{
			return new TableLayout(new TableRow(cells));
		}

		/// <summary>
		/// Creates a horizontal table layout with the specified cells scaled equally.
		/// </summary>
		/// <remarks>
		/// Since table layouts are by default vertical by defining the rows and the cells for each row,
		/// it is verbose to create nested tables when you want a horizontal table.  E.g. <code>new TableLayout(new TableRow(...))</code>.
		/// 
		/// This method is used to easily create a single row table layout with a horizontal set of cells. E.g.
		/// <code>TableLayout.HorizontalScaled(...)</code>
		/// 
		/// The difference between Horizontal and HorizontalScaled is that this method sets
		/// ScaleWidth on each cell.
		/// </remarks>
		/// <param name="cells">Cells for the row</param>
		/// <returns>A new single row table layout with the specified cells</returns>
		public static TableLayout HorizontalScaled(params TableCell[] cells)
		{
			foreach (TableCell cell in cells)
				if(cell != null)
					cell.ScaleWidth = true;
			return new TableLayout(new TableRow(cells));
		}

		/// <summary>
		/// Creates a horizontal table layout with the specified cells.
		/// </summary>
		/// <remarks>
		/// Since table layouts are by default vertical by defining the rows and the cells for each row,
		/// it is verbose to create nested tables when you want a horizontal table.  E.g. <code>new TableLayout(new TableRow(...))</code>.
		/// 
		/// This method is used to easily create a single row table layout with a horizontal set of cells. E.g.
		/// <code>TableLayout.Horizontal(...)</code>
		/// </remarks>
		/// <param name="spacing">Spacing between cells</param>
		/// <param name="cells">Cells for the row</param>
		/// <returns>A new single row table layout with the specified cells</returns>
		public static TableLayout Horizontal(int spacing, params TableCell[] cells)
		{
			return new TableLayout(new TableRow(cells)) { Spacing = new Size(spacing, spacing) };
		}

		/// <summary>
		/// Creates a horizontal table layout with the specified cells scaled equally.
		/// </summary>
		/// <remarks>
		/// Since table layouts are by default vertical by defining the rows and the cells for each row,
		/// it is verbose to create nested tables when you want a horizontal table.  E.g. <code>new TableLayout(new TableRow(...))</code>.
		/// 
		/// This method is used to easily create a single row table layout with a horizontal set of cells. E.g.
		/// <code>TableLayout.HorizontalScaled(...)</code>
		/// 
		/// The difference between Horizontal and HorizontalScaled is that this method sets
		/// ScaleWidth on each cell.
		/// </remarks>
		/// <param name="spacing">Spacing between cells</param>
		/// <param name="cells">Cells for the row</param>
		/// <returns>A new single row table layout with the specified cells</returns>
		public static TableLayout HorizontalScaled(int spacing, params TableCell[] cells)
		{
			foreach (TableCell cell in cells)
				if(cell != null)
					cell.ScaleWidth = true;
			return new TableLayout(new TableRow(cells)) { Spacing = new Size(spacing, spacing) };
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class.
		/// </summary>
		public TableLayout()
		{
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class with the specified number of columns and rows.
		/// </summary>
		/// <param name="columns">Number of columns in the table.</param>
		/// <param name="rows">Number of rows in the table.</param>
		public TableLayout(int columns, int rows)
			: this(new Size(columns, rows))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class with the specified dimensions.
		/// </summary>
		/// <param name="dimensions">Dimensions of the table.</param>
		public TableLayout(Size dimensions)
		{
			SetCellSize(dimensions, true);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class with the specified rows.
		/// </summary>
		/// <param name="rows">Rows to populate the table.</param>
		public TableLayout(params TableRow[] rows)
		{
			Rows = new TableRowCollection(this, rows);
			Create();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class with the specified rows.
		/// </summary>
		/// <param name="rows">Rows to populate the table.</param>
		public TableLayout(IEnumerable<TableRow> rows)
		{
			Rows = new TableRowCollection(this, rows);
			Create();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableLayout"/> class with the specified rows.
		/// </summary>
		/// <param name="yscale">Scale all rows</param>
		/// <param name="rows">Rows to populate the table.</param>
		public TableLayout(bool yscale, params TableRow[] rows)
		{
			if (yscale)
				foreach (TableRow row in rows)
					if(row != null)
						row.ScaleHeight = true;
			Rows = new TableRowCollection(this, rows);
			Create();
			Initialize();
		}

		void Create()
		{
			var rows = Rows;
			var columnCount = rows.DefaultIfEmpty().Max(r => r != null ? r.Cells.Count : 0);
			SetCellSize(new Size(columnCount, rows.Count), false);
			if (columnCount > 0)
			{
				for (int y = 0; y < rows.Count; y++)
				{
					var row = rows[y];
					while (row.Cells.Count < columnCount)
						row.Cells.Add(new TableCell());
					for (int x = 0; x < columnCount; x++)
					{
						var cell = row.Cells[x];
						Add(cell.Control, x, y);
						if (cell.ScaleWidth)
							SetColumnScale(x);
					}
					while (row.Cells.Count < columnCount)
						row.Cells.Add(new TableCell());
					if (row.ScaleHeight)
						SetRowScale(y);
				}
			}
		}

		void SetCellSize(Size value, bool createRows)
		{
			if (created)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can only set the cell size of a table once"));
			dimensions = value;
			Handler.CreateControl(dimensions.Width, dimensions.Height);
			if (!dimensions.IsEmpty)
			{
				if (createRows)
				{
					var rows = Enumerable.Range(0, value.Height).Select(r => new TableRow(Enumerable.Range(0, value.Width).Select(c => new TableCell())));
					Rows = new TableRowCollection(this, rows);
				}
			}
			created = true;
		}

		/// <summary>
		/// Sets the scale for the specified column.
		/// </summary>
		/// <param name="column">Column to set the scale for.</param>
		/// <param name="scale">If set to <c>true</c> scale, otherwise size to preferred size of controls in the column.</param>
		public void SetColumnScale(int column, bool scale = true)
		{
			Handler.SetColumnScale(column, scale);
		}

		/// <summary>
		/// Gets the scale for the specified column.
		/// </summary>
		/// <returns><c>true</c>, if column is scaled, <c>false</c> otherwise.</returns>
		/// <param name="column">Column to retrieve the scale.</param>
		public bool GetColumnScale(int column)
		{
			return Handler.GetColumnScale(column);
		}

		/// <summary>
		/// Sets the scale for the specified row.
		/// </summary>
		/// <param name="row">Row to set the scale for.</param>
		/// <param name="scale">If set to <c>true</c> scale, otherwise size to preferred size of controls in the row.</param>
		public void SetRowScale(int row, bool scale = true)
		{
			Handler.SetRowScale(row, scale);
		}

		/// <summary>
		/// Gets the scale for the specified row.
		/// </summary>
		/// <returns><c>true</c>, if row is scaled, <c>false</c> otherwise.</returns>
		/// <param name="row">Row to retrieve the scale.</param>
		public bool GetRowScale(int row)
		{
			return Handler.GetRowScale(row);
		}

		/// <summary>
		/// Adds a control to the specified x &amp; y coordinates.
		/// </summary>
		/// <remarks>
		/// If a control already exists in the location, it is replaced. Only one control can exist in a cell.
		/// </remarks>
		/// <param name="control">Control to add.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void Add(Control control, int x, int y)
		{
			InnerAdd(control, x, y);
		}

		void InnerAdd(Control control, int x, int y)
		{
			if (dimensions.IsEmpty)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "You must set the size of the TableLayout before adding controls"));
			var cell = Rows[y].Cells[x];

			SetParent(control, () => {
				cell.Control = control;
				Handler.Add(control, x, y);
			}, cell.Control);
		}

		/// <summary>
		/// Adds a control to the specified x &amp; y coordinates.
		/// </summary>
		/// <remarks>
		/// If a control already exists in the location, it is replaced. Only one control can exist in a cell.
		/// The <paramref name="xscale"/> and <paramref name="yscale"/> parameters are to easily set the scaling
		/// for the current row/column while adding the control.
		/// </remarks>
		/// <param name="control">Control to add.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="xscale">If set to <c>true</c> xscale.</param>
		/// <param name="yscale">If set to <c>true</c> yscale.</param>
		public void Add(Control control, int x, int y, bool xscale, bool yscale)
		{
			SetColumnScale(x, xscale);
			SetRowScale(y, yscale);
			Add(control, x, y);
		}

		/// <summary>
		/// Adds a control to the specified location.
		/// </summary>
		/// <remarks>
		/// If a control already exists in the location, it is replaced. Only one control can exist in a cell.
		/// </remarks>
		/// <param name="control">Control to add.</param>
		/// <param name="location">The location of the control.</param>
		public void Add(Control control, Point location)
		{
			Add(control, location.X, location.Y);
		}

		/// <summary>
		/// Moves the specified control to the new x and y coordinates.
		/// </summary>
		/// <remarks>
		/// If a control already exists in the new location, it will be replaced. Only one control can exist in a cell.
		/// The old location of the control will have an empty space.
		/// </remarks>
		/// <param name="control">Control to move.</param>
		/// <param name="x">The new x coordinate.</param>
		/// <param name="y">The new y coordinate.</param>
		public void Move(Control control, int x, int y)
		{
			var cell = Rows.SelectMany(r => r.Cells).FirstOrDefault(r => r.Control == control);
			if (cell != null)
				cell.Control = null;

			cell = Rows[y].Cells[x];
			var old = cell.Control;
			if (old != null)
				RemoveParent(old);
			cell.Control = control;
			Handler.Move(control, x, y);
		}

		/// <summary>
		/// Move the specified control to a new location.
		/// </summary>
		/// <remarks>
		/// If a control already exists in the new location, it will be replaced. Only one control can exist in a cell.
		/// The old location of the control will have an empty space.
		/// </remarks>
		/// <param name="control">Control to move.</param>
		/// <param name="location">New location of the control.</param>
		public void Move(Control control, Point location)
		{
			Move(control, location.X, location.Y);
		}

		/// <summary>
		/// Remove the specified child control.
		/// </summary>
		/// <param name="child">Child control to remove.</param>
		public override void Remove(Control child)
		{
			var rows = Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				var row = rows[i];
				for (int c = 0; c < row.Cells.Count; c++)
				{
					var cell = row.Cells[c];
					if (ReferenceEquals(cell.Control, child))
					{
						cell.SetControl(null);
						Handler.Remove(child);
						RemoveParent(child);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the horizontal and vertical spacing between each of the cells of the table.
		/// </summary>
		/// <value>The spacing between the cells.</value>
		public Size Spacing
		{
			get { return Handler.Spacing; }
			set { Handler.Spacing = value; }
		}

		/// <summary>
		/// Gets or sets the padding bordering the table.
		/// </summary>
		/// <value>The padding bordering the table.</value>
		public Padding Padding
		{
			get { return Handler.Padding; }
			set { Handler.Padding = value; }
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			OnDeserialized(false);
		}

		/// <summary>
		/// Ends the initialization when loading from xaml or other code generated scenarios
		/// </summary>
		public override void EndInit()
		{
			base.EndInit();
			OnDeserialized(VisualParent != null); // mono calls EndInit BEFORE setting to parent
		}

		/// <summary>
		/// Raises the <see cref="Control.PreLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnPreLoad(EventArgs e)
		{
			OnDeserialized(true);
			base.OnPreLoad(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.Load"/> event, and recursed to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			// ensure we've been deserialized, in case something was done in load or pre-load event
			OnDeserialized(false);
		}

		void OnDeserialized(bool direct)
		{
			if (Loaded || direct)
			{
				if (!created)
				{
					var rows = Properties.Get<Collection<TableRow>>(rowsKey);
					if (rows != null)
					{
						Create();
					}
				}
			}
		}

		/// <summary>
		/// Handler interface for <see cref="TableLayout"/>
		/// </summary>
		/// <remarks>
		/// Currently, TableLayout handlers only need to set its size while created and cannot be resized.
		/// </remarks>
		[AutoInitialize(false)]
		public new interface IHandler : Layout.IHandler, IPositionalLayoutHandler
		{
			/// <summary>
			/// Creates the control with the specified dimensions.
			/// </summary>
			/// <param name="columns">Number of columns for the table.</param>
			/// <param name="rows">Number of rows for the table.</param>
			void CreateControl(int columns, int rows);

			/// <summary>
			/// Gets the scale for the specified column.
			/// </summary>
			/// <returns><c>true</c>, if column is scaled, <c>false</c> otherwise.</returns>
			/// <param name="column">Column to retrieve the scale.</param>
			bool GetColumnScale(int column);

			/// <summary>
			/// Sets the scale for the specified column.
			/// </summary>
			/// <param name="column">Column to set the scale for.</param>
			/// <param name="scale">If set to <c>true</c> scale, otherwise size to preferred size of controls in the column.</param>
			void SetColumnScale(int column, bool scale);

			/// <summary>
			/// Gets the scale for the specified row.
			/// </summary>
			/// <returns><c>true</c>, if row is scaled, <c>false</c> otherwise.</returns>
			/// <param name="row">Row to retrieve the scale.</param>
			bool GetRowScale(int row);

			/// <summary>
			/// Sets the scale for the specified row.
			/// </summary>
			/// <param name="row">Row to set the scale for.</param>
			/// <param name="scale">If set to <c>true</c> scale, otherwise size to preferred size of controls in the row.</param>
			void SetRowScale(int row, bool scale);

			/// <summary>
			/// Gets or sets the horizontal and vertical spacing between each of the cells of the table.
			/// </summary>
			/// <value>The spacing between the cells.</value>
			Size Spacing { get; set; }

			/// <summary>
			/// Gets or sets the padding bordering the table.
			/// </summary>
			/// <value>The padding bordering the table.</value>
			Padding Padding { get; set; }
		}

		/// <summary>
		/// Implicitly converts an array of rows to a vertical TableLayout
		/// </summary>
		/// <param name="rows">Rows to convert.</param>
		public static implicit operator TableLayout(TableRow[] rows)
		{
			return new TableLayout(rows);
		}

		internal void InternalSetLogicalParent(Control control)
		{
			if (control?.InternalLogicalParent == null)
				SetLogicalParent(control);
		}

		internal void InternalRemoveLogicalParent(Control control)
		{
			if (ReferenceEquals(control?.InternalLogicalParent, this))
				RemoveLogicalParent(control);
		}
	}
}
