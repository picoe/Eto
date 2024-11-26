namespace Eto.Forms.ThemedControls;

/// <summary>
/// A themed handler for the deprecated <see cref="TreeView"/> control, implemented using a <see cref="TreeGridView"/>
/// with a single image/text column.
/// </summary>
[Obsolete("Since 2.4. TreeView is deprecated, please use TreeGridView instead.")]
public class ThemedTreeViewHandler : ThemedControlHandler<TreeGridView, TreeView, TreeView.ICallback>, TreeView.IHandler
{
	ITreeStore _dataStore;
	TreeStoreAdapter _adapter;
	bool _labelEdit;
	Color _textColor;

	// Wraps an ITreeItem as an ITreeGridItem so it can be used with TreeGridView.
	// Also implements ITreeGridStore<ITreeGridItem> so TreeGridView can access children.
	sealed class TreeItemWrapper : ITreeGridItem, ITreeGridStore<ITreeGridItem>
	{
		internal readonly ITreeItem Inner;
		readonly Dictionary<ITreeItem, TreeItemWrapper> _cache;

		public TreeItemWrapper(ITreeItem inner, Dictionary<ITreeItem, TreeItemWrapper> cache)
		{
			Inner = inner;
			_cache = cache;
		}

		public bool Expanded
		{
			get => Inner.Expanded;
			set => Inner.Expanded = value;
		}

		public bool Expandable => Inner.Expandable;

		public ITreeGridItem Parent { get; set; }

		public string Text => Inner.Text;

		public Image Image => Inner.Image;

		TreeItemWrapper GetOrCreate(ITreeItem item)
		{
			if (!_cache.TryGetValue(item, out var wrapper))
			{
				wrapper = new TreeItemWrapper(item, _cache) { Parent = this };
				_cache[item] = wrapper;
			}
			return wrapper;
		}

		public ITreeGridItem this[int index] => GetOrCreate(Inner[index]);

		public int Count => Inner.Count;
	}

	// Adapts an ITreeStore to ITreeGridStore<ITreeGridItem> for the TreeGridView data store.
	sealed class TreeStoreAdapter : ITreeGridStore<ITreeGridItem>
	{
		readonly ITreeStore _store;
		internal readonly Dictionary<ITreeItem, TreeItemWrapper> Cache = new Dictionary<ITreeItem, TreeItemWrapper>();

		public TreeStoreAdapter(ITreeStore store) => _store = store;

		internal TreeItemWrapper GetOrCreate(ITreeItem item)
		{
			if (!Cache.TryGetValue(item, out var wrapper))
			{
				wrapper = new TreeItemWrapper(item, Cache);
				Cache[item] = wrapper;
			}
			return wrapper;
		}

		public ITreeGridItem this[int index] => GetOrCreate(_store[index]);

		public int Count => _store.Count;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ThemedTreeViewHandler"/>.
	/// </summary>
	public ThemedTreeViewHandler()
	{
		Control = new TreeGridView { ShowHeader = false };
		Control.Columns.Add(new GridColumn
		{
			AutoSize = true,
			Expand = true,
			Editable = false,
			DataCell = new ImageTextCell
			{
				TextBinding = new DelegateBinding<TreeItemWrapper, string>(w => w.Text, (w, v) => w.Inner.Text = v),
				ImageBinding = new DelegateBinding<TreeItemWrapper, Image>(w => w.Image)
			}
		});
	}

	ITreeItem Unwrap(ITreeGridItem item) => (item as TreeItemWrapper)?.Inner;

	ITreeGridItem Wrap(ITreeItem item) => item == null ? null : _adapter?.GetOrCreate(item);

	/// <inheritdoc/>
	public ITreeStore DataStore
	{
		get => _dataStore;
		set
		{
			_dataStore = value;
			_adapter = value != null ? new TreeStoreAdapter(value) : null;
			Control.DataStore = _adapter;
		}
	}

	/// <inheritdoc/>
	public ITreeItem SelectedItem
	{
		get => Unwrap(Control.SelectedItem);
		set => Control.SelectedItem = Wrap(value);
	}

	/// <inheritdoc/>
	public Color TextColor
	{
		get => _textColor;
		set => _textColor = value;
	}

	/// <inheritdoc/>
	public void RefreshData() => Control.ReloadData();

	/// <inheritdoc/>
	public void RefreshItem(ITreeItem item)
	{
		var wrapped = Wrap(item);
		if (wrapped != null)
			Control.ReloadItem(wrapped);
	}

	/// <inheritdoc/>
	public ITreeItem GetNodeAt(PointF point)
	{
		var cell = Control.GetCellAt(point);
		return Unwrap(cell?.Item as ITreeGridItem);
	}

	/// <inheritdoc/>
	public bool LabelEdit
	{
		get => _labelEdit;
		set
		{
			if (_labelEdit != value)
			{
				_labelEdit = value;
				if (Control.Columns.Count > 0)
					Control.Columns[0].Editable = value;
			}
		}
	}

	/// <inheritdoc/>
	public new ContextMenu ContextMenu
	{
		get => Control.ContextMenu;
		set => Control.ContextMenu = value;
	}

	/// <inheritdoc/>
	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case TreeView.SelectionChangedEvent:
				Control.SelectionChanged += (sender, e) =>
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				break;

			case TreeView.ActivatedEvent:
				Control.Activated += (sender, e) =>
				{
					var item = Unwrap(e.Item);
					if (item != null)
						Callback.OnActivated(Widget, new TreeViewItemEventArgs(item));
				};
				break;

			case TreeView.ExpandingEvent:
				Control.Expanding += (sender, e) =>
				{
					var item = Unwrap(e.Item);
					if (item != null)
					{
						var args = new TreeViewItemCancelEventArgs(item);
						Callback.OnExpanding(Widget, args);
						e.Cancel = args.Cancel;
					}
				};
				break;

			case TreeView.ExpandedEvent:
				Control.Expanded += (sender, e) =>
				{
					var item = Unwrap(e.Item);
					if (item != null)
						Callback.OnExpanded(Widget, new TreeViewItemEventArgs(item));
				};
				break;

			case TreeView.CollapsingEvent:
				Control.Collapsing += (sender, e) =>
				{
					var item = Unwrap(e.Item);
					if (item != null)
					{
						var args = new TreeViewItemCancelEventArgs(item);
						Callback.OnCollapsing(Widget, args);
						e.Cancel = args.Cancel;
					}
				};
				break;

			case TreeView.CollapsedEvent:
				Control.Collapsed += (sender, e) =>
				{
					var item = Unwrap(e.Item);
					if (item != null)
						Callback.OnCollapsed(Widget, new TreeViewItemEventArgs(item));
				};
				break;

			case TreeView.LabelEditingEvent:
				Control.CellEditing += (sender, e) =>
				{
					var item = Unwrap(e.Item as ITreeGridItem);
					if (item != null)
						Callback.OnLabelEditing(Widget, new TreeViewItemCancelEventArgs(item));
				};
				break;

			case TreeView.LabelEditedEvent:
				Control.CellEdited += (sender, e) =>
				{
					var item = Unwrap(e.Item as ITreeGridItem);
					if (item != null)
					{
						var args = new TreeViewItemEditEventArgs(item, item.Text);
						Callback.OnLabelEdited(Widget, args);
					}
				};
				break;

			case TreeView.NodeMouseClickEvent:
				Control.MouseDown += (sender, e) =>
				{
					var cell = Control.GetCellAt(e.Location);
					var item = Unwrap(cell?.Item as ITreeGridItem);
					if (item != null)
						Callback.OnNodeMouseClick(Widget, new TreeViewItemEventArgs(item));
				};
				break;

			default:
				base.AttachEvent(id);
				break;
		}
	}
}
