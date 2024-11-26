namespace Eto.Forms.ThemedControls;

/// <summary>
/// A themed handler for the <see cref="ListBox"/> control, implemented using a <see cref="GridView"/>
/// with a single image/text column.
/// </summary>
public class ThemedListBoxHandler : ThemedControlHandler<GridView, ListBox, ListBox.ICallback>, ListBox.IHandler
{
	IEnumerable<object> _dataStore;
	IIndirectBinding<string> _itemTextBinding;
	IIndirectBinding<string> _itemKeyBinding;
	IIndirectBinding<Image> _itemImageBinding;
	Color? _textColor;
	Font _font;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThemedListBoxHandler"/>.
	/// </summary>
	public ThemedListBoxHandler()
	{
		Control = new GridView { ShowHeader = false, AllowMultipleSelection = false };
		Control.Columns.Add(new GridColumn
		{
			AutoSize = true,
			Expand = true,
			Editable = false,
			DataCell = new ImageTextCell
			{
				TextBinding = new DelegateBinding<object, string>(item => GetItemText(item), SetItemText),
				ImageBinding = new DelegateBinding<object, Image>(item => GetItemImage(item))
			}
		});

		// Wire up events
		Control.SelectionChanged += (sender, e) =>
			Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);

		Control.CellDoubleClick += (sender, e) =>
			Callback.OnActivated(Widget, EventArgs.Empty);
			
		Control.CellFormatting += (sender, e) =>
		{
			if (_textColor != null)
				e.ForegroundColor = TextColor;
			if (_font != null)
				e.Font = Font;
		};
	}

	private Image GetItemImage(object item)
	{
		if (_itemImageBinding == null || item == null)
			return null;
		return _itemImageBinding.GetValue(item);
	}

	private string GetItemText(object item)
	{
		if (item == null)
			return null;
		if (_itemTextBinding != null)
			return _itemTextBinding.GetValue(item);
		return item.ToString();
	}

	private void SetItemText(object item, string value)
	{
		if (item != null && _itemTextBinding != null)
			_itemTextBinding.SetValue(item, value);
	}

	/// <inheritdoc/>
	public IEnumerable<object> DataStore
	{
		get => _dataStore;
		set
		{
			_dataStore = value;
			Control.DataStore = value;
		}
	}

	/// <inheritdoc/>
	public int SelectedIndex
	{
		get => Control.SelectedRow;
		set => Control.SelectedRow = value;
	}

	/// <inheritdoc/>
	public Color TextColor
	{
		get => _textColor ?? SystemColors.ControlText;
		set => _textColor = value;
	}

	/// <inheritdoc/>
	public Font Font
	{
		get => _font ?? SystemFonts.Default();
		set => _font = value;
	}

	/// <inheritdoc/>
	public IIndirectBinding<string> ItemTextBinding
	{
		get => _itemTextBinding ?? new ListItemTextBinding();
		set
		{
			_itemTextBinding = value;
			// Update the cell binding
			if (Control.Columns.Count > 0 && Control.Columns[0].DataCell is ImageTextCell cell)
			{
				cell.TextBinding = Binding.Delegate<object, string>(GetItemText, SetItemText);
			}
		}
	}

	/// <inheritdoc/>
	public IIndirectBinding<string> ItemKeyBinding
	{
		get => _itemKeyBinding ?? new ListItemKeyBinding();
		set => _itemKeyBinding = value;
	}

	/// <inheritdoc/>
	public IIndirectBinding<Image> ItemImageBinding
	{
		get => _itemImageBinding;
		set
		{
			_itemImageBinding = value;
			// Update the cell binding
			if (Control.Columns.Count > 0 && Control.Columns[0].DataCell is ImageTextCell cell)
			{
				cell.ImageBinding = value != null
					? Binding.Delegate((object item) => GetItemImage(item))
					: null;
			}
		}
	}

	/// <inheritdoc/>
	public BorderType Border
	{
		get => Control.Border;
		set => Control.Border = value;
	}

	/// <inheritdoc/>
	public new ContextMenu ContextMenu
	{
		get => Control.ContextMenu;
		set => Control.ContextMenu = value;
	}
}
