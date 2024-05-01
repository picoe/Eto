namespace Eto.Forms;

/// <summary>
/// Represents a cell in a <see cref="TableRow"/>
/// </summary>
[ContentProperty("Control")]
[sc.TypeConverter(typeof(TableCellConverter))]
public class TableCell
{
	Control _control;
	TableCellCollection _cells;

	TableLayout ParentTable => _cells?._layout;

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TableCell"/> will scale its width
	/// </summary>
	/// <remarks>
	/// All controls in the same column of this cell will get the same scaling value.
	/// Scaling will make the column expand to fit the rest of the width of the container, minus the preferred
	/// width of any non-scaled columns.
	/// 
	/// If there are no columns with width scaling, the last column will automatically get scaled.
	/// 
	/// With scaling turned off, cells in the column will fit the preferred size of the widest control.
	/// </remarks>
	/// <value><c>true</c> if scale width; otherwise, <c>false</c>.</value>
	public bool ScaleWidth { get; set; }

	/// <summary>
	/// Gets or sets the control in this cell, or null for an empty space
	/// </summary>
	/// <value>The control.</value>
	public Control Control
	{
		get { return _control; }
		set
		{
			if (_control != value)
			{
				value?.Detach();
				_control?.Detach();
				var layout = ParentTable;
				layout?.InternalRemoveLogicalParent(_control);
				_control = value;
				layout?.InternalSetLogicalParent(_control);
			}
		}
	}

	internal void SetControl(Control control)
	{
		_control = control;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TableCell"/> class.
	/// </summary>
	public TableCell()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TableCell"/> class.
	/// </summary>
	/// <param name="control">Control for this cell</param>
	/// <param name="scaleWidth">Scale the width of the control if <c>true</c>, otherwise scale to the preferred size of the control.</param>
	public TableCell(Control control, bool scaleWidth = false)
	{
		Control = control;
		ScaleWidth = scaleWidth;
	}

	/// <summary>
	/// Converts a control to a table cell
	/// </summary>
	/// <param name="control">Control to convert to a cell.</param>
	public static implicit operator TableCell(Control control)
	{
		return new TableCell(control);
	}

	/// <summary>
	/// Converts an array of cells to a new cell with a table of vertical cells in a new child TableLayout
	/// </summary>
	/// <param name="items">Items to convert.</param>
	public static implicit operator TableCell(TableCell[] items)
	{
		return new TableCell(new TableLayout(items));
	}

	/// <summary>
	/// Converts an array of rows to a new cell with vertical rows in a new child TableLayout
	/// </summary>
	/// <param name="rows">Rows to convert.</param>
	public static implicit operator TableCell(TableRow[] rows)
	{
		return new TableCell(new TableLayout(rows));
	}

	/// <summary>
	/// Converts a string to a TableCell with a label control implicitly.
	/// </summary>
	/// <remarks>
	/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
	/// </remarks>
	/// <param name="labelText">Text to convert to a Label control.</param>
	public static implicit operator TableCell(string labelText)
	{
		return new TableCell(new Label { Text = labelText });
	}

	/// <summary>
	/// Converts an <see cref="Image"/> to a TableCell with an <see cref="ImageView"/> control implicitly.
	/// </summary>
	/// <remarks>
	/// This provides an easy way to add images to your layout through code, without having to create <see cref="ImageView"/> instances manually.
	/// </remarks>
	/// <param name="image">Image to convert to a TableCell with an ImageView control.</param>
	public static implicit operator TableCell(Image image)
	{
		return new TableCell(new ImageView { Image = image });
	}

	internal void SetLayout(TableCellCollection cells, bool shouldRemove, TableLayout oldLayout = null)
	{
		if (!ReferenceEquals(_cells, cells))
		{
			// remove from old cells collection
			if (shouldRemove)
				_cells?.RemoveItemWithoutLayoutUpdate(this);
				
			ParentTable?.InternalRemoveLogicalParent(_control);
			_cells = cells;
			ParentTable?.InternalSetLogicalParent(_control);
		}
		else if (!ReferenceEquals(oldLayout, ParentTable))
		{
			oldLayout?.InternalRemoveLogicalParent(_control);
			ParentTable?.InternalSetLogicalParent(_control);
		}
	}
}

class TableCellCollection : Collection<TableCell>, IList
{
	internal TableLayout _layout;

	public TableCellCollection()
	{
	}

	public TableCellCollection(IEnumerable<TableCell> list)
		: base(list.Select(r => r ?? new TableCell { ScaleWidth = true }).ToList())
	{
	}
	
	internal void SetLayout(TableRow row, TableLayout layout, bool shouldRemove)
	{
		if (ReferenceEquals(layout, _layout))
			return;

		var oldLayout = _layout;
		if (shouldRemove && _layout?.Rows is TableRowCollection oldLayoutRows)
		{
			// switching layouts, remove from old layout without triggering a new SetLayout
			// This really shouldn't be done and should throw an exception
			oldLayoutRows.RemoveItemWithoutLayoutUpdate(row);
		}
		_layout = layout;
		
		foreach (var cell in this)
			cell.SetLayout(this, true, oldLayout);
	}
	
	internal void RemoveItemWithoutLayoutUpdate(TableCell cell)
	{
		var index = IndexOf(cell);
		if (index >= 0)
			base.RemoveItem(index);
	}


	protected override void RemoveItem(int index)
	{
		var item = this[index];
		item?.SetLayout(null, false);
		base.RemoveItem(index);
	}

	protected override void ClearItems()
	{
		foreach (var item in this)
		{
			item?.SetLayout(null, false);
		}
		base.ClearItems();
	}

	protected override void InsertItem(int index, TableCell item)
	{
		if (item == null)
			item = new TableCell { ScaleWidth = true };
		item.SetLayout(this, true);
		base.InsertItem(index, item);
	}

	protected override void SetItem(int index, TableCell item)
	{
		var old = this[index];
		old?.SetLayout(null, false);
		if (item == null)
			item = new TableCell { ScaleWidth = true };
		item.SetLayout(this, true);
		base.SetItem(index, item);
	}

	int IList.Add(object value)
	{
		// allow adding a control directly from xaml
		if (value is Control control)
			Add(control);
		else if (value is string str)
			Add(new Label { Text = str });
		else
			Add((TableCell)value);
		return Count - 1;
	}
}