using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Item store for the <see cref="TreeGridView"/>
	/// </summary>
	public interface ITreeGridStore<out T> : IDataStore<T>
		where T: ITreeGridItem
	{
	}

	/// <summary>
	/// Event arguments for <see cref="TreeGridView"/> events
	/// </summary>
	public class TreeGridViewItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the item that triggered the event.
		/// </summary>
		/// <value>The item that triggered the event.</value>
		public ITreeGridItem Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridViewItemEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item that triggered the event.</param>
		public TreeGridViewItemEventArgs(ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	/// <summary>
	/// Event arguments for <see cref="TreeGridView"/> events that can be cancelled
	/// </summary>
	public class TreeGridViewItemCancelEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Gets the item that triggered the event.
		/// </summary>
		/// <value>The item that triggered the event.</value>
		public ITreeGridItem Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridViewItemCancelEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item that triggered the event.</param>
		public TreeGridViewItemCancelEventArgs(ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	/// <summary>
	/// Information of a cell in the <see cref="TreeGridView"/>
	/// </summary>
	public class TreeGridCell
	{
		/// <summary>
		/// Gets the item associated with the row of the cell.
		/// </summary>
		/// <value>The row item.</value>
		public object Item { get; }

		/// <summary>
		/// Gets the column of the cell, or null
		/// </summary>
		/// <value>The column.</value>
		public GridColumn Column { get; }

		/// <summary>
		/// Gets the index of the column.
		/// </summary>
		/// <value>The index of the column.</value>
		public int ColumnIndex { get; }

		internal TreeGridCell(object item, GridColumn column, int columnIndex)
		{
			Item = item;
			Column = column;
			ColumnIndex = columnIndex;
		}
	}

	/// <summary>
	/// Enumeration of the drag position relative to a node or item in a Grid.
	/// </summary>
	public enum GridDragPosition
	{
		/// <summary>
		/// The user is dragging overtop an existing node or item.
		/// </summary>
		Over,
		/// <summary>
		/// The user is dragging to insert before a node or item.
		/// </summary>
		Before,
		/// <summary>
		/// The user is dragging to insert after a node or item.
		/// </summary>
		After
	}

	/// <summary>
	/// Extra drag information when dragging to a <see cref="TreeGridView"/>.
	/// </summary>
	/// <remarks>
	/// Use this information to determine where the user is dragging to, and also to change where the drag indicator will
	/// be shown by modifying the Item and ChildIndex properties.
	/// </remarks>
	public class TreeGridViewDragInfo
	{
		object _item;
		GridDragPosition _position;
		int? _childIndex;
		object _parent;

		/// <summary>
		/// Gets or sets the parent node of the <see cref="Item"/> to drag to.
		/// </summary>
		/// <remarks>
		/// Normally you would only need to set <see cref="Item"/> to specify which node to drag to.
		/// 
		/// However, in the case of dragging to below the Parent as inserting a first child you would set Parent to the node, 
		/// <see cref="Item"/> to null, and <see cref="Position"/> to <see cref="GridDragPosition.After"/>.
		/// </remarks>
		public object Parent
		{
			get { return _parent; }
			set
			{
				if (!ReferenceEquals(_parent, value))
				{
					_parent = value;
					_childIndex = null;
					IsChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the index of the <see cref="Item"/> relative to the <see cref="Parent"/>.
		/// </summary>
		public int ChildIndex
		{
			get
			{
				if (_childIndex == null)
				{
					object list = Parent ?? Control.DataStore;

					var parentList = list as IList;
					if (parentList != null)
					{
						_childIndex = parentList.IndexOf(_item);
					}
					else
					{
						var parentStore = list as ITreeGridStore<ITreeGridItem>;
						if (parentStore != null)
						{
							for (int i = 0; i < parentStore.Count; i++)
							{
								if (ReferenceEquals(parentStore[i], _item))
								{
									_childIndex = i;
									break;
								}
							}
						}
						else
							_childIndex = -1;
					}
				}
				return _childIndex.Value;
			}
		}

		/// <summary>
		/// Gets or sets the insertion index where the user is dragging to as a child of Item, or -1 if dragging over the Item.
		/// </summary>
		/// <remarks>
		/// This is useful if the user is dragging between existing items, or the beginning or end of a child list.
		/// </remarks>
		/// <value>The insertion index where the user is dragging to, otherwise -1 if dragging over an item.</value>
		public int InsertIndex
		{
			get
			{
				if (Position == GridDragPosition.Before)
					return ChildIndex;
				if (Position == GridDragPosition.After)
				{
					if (ReferenceEquals(Item, null))
					{
						if (ReferenceEquals(Parent, null))
							return -1;
						if (ChildIndex == -1)
							return 0;
						return ChildIndex;
					}
					return ChildIndex + 1;
				}
				return -1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the drop should insert before, after, or over the <see cref="Item"/>.
		/// </summary>
		/// <remarks>
		/// When this is Before or After, you can use the <see cref="InsertIndex"/> to determine what index to insert the 
		/// item as a child of the <see cref="Parent"/> node.
		/// </remarks>
		/// <value>The position to insert the dropped item, or over.</value>
		public GridDragPosition Position
		{
			get { return _position; }
			set
			{
				if (_position != value)
				{
					_position = value;
					IsChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the parent tree control this info is for.
		/// </summary>
		/// <value>The parent tree for the drag info.</value>
		public TreeGridView Control { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.TreeGridDragInfo"/> is changed.
		/// </summary>
		/// <remarks>
		/// This will return true if the <see cref="InsertIndex"/> or <see cref="Parent"/> have been set.
		/// This is useful for platform implementations to determine if the drop target has been modified.
		/// </remarks>
		/// <value><c>true</c> if is changed; otherwise, <c>false</c>.</value>
		public bool IsChanged { get; private set; }

		/// <summary>
		/// Gets or sets the item to drag to, or null if dragging below the <see cref="Parent"/> node.
		/// </summary>
		/// <remarks>
		/// This specifies the target item to drag to.  Note that if <see cref="Position"/> is Before or After,
		/// then you should use <see cref="InsertIndex"/> to insert the nodes at that specified location.
		/// 
		/// If you do not want to allow inserting, use <see cref="RestrictToOver"/> in the <see cref="Control.DragOver"/> event,
		/// or you can also use <see cref="RestrictToInsert"/> to only allow inserting items.
		/// </remarks>
		public object Item
		{
			get { return _item; }
			set
			{
				if (!ReferenceEquals(_item, value))
				{
					_item = value;
					_parent = (_item as ITreeGridItem)?.Parent;
					_childIndex = null;

					IsChanged = true;
				}
			}
		}

		/// <summary>
		/// Helper to restrict to drop on top an existing item without allowing any insertion.
		/// </summary>
		public void RestrictToOver()
		{
			Item = Item ?? Parent;
			Position = GridDragPosition.Over;
		}

		/// <summary>
		/// Helper to restrict the drop to insert items only without allowing draging over existing items.
		/// </summary>
		public void RestrictToInsert()
		{
			if (Position == GridDragPosition.Over)
				Position = GridDragPosition.Before;
		}

		/// <summary>
		/// Restricts the drop to an item or a child within the specified number of levels.
		/// </summary>
		/// <param name="item">Item to restrict the drop to, or any of its children</param>
		/// <param name="childLevels">Number of child levels to allow, or -1 to allow any number of levels</param>
		/// <returns>True if the drag was restricted, or false if the user is already dragging over the specified item or its children.</returns>
		public bool RestrictToNode(object item, int childLevels = -1)
		{
			var child = (Item ?? Parent) as ITreeGridItem;
			if (ReferenceEquals(item, child) && Position != GridDragPosition.Over)
			{
				// already over the specified item node, force position to over
				Position = GridDragPosition.Over;
				return true;
			}

			// go up parent chain to ensure we're a descendent of the item node
			while (child != null && !ReferenceEquals(item, child))
			{
				if (childLevels == 0)
					break;
				if (childLevels > 0)
					childLevels--;
				child = child.Parent;
			}

			// was a child of (or equal to) the item, so allow it.
			if (ReferenceEquals(item, child))
				return false;
			// not a child of the specified item, so the drag will drop on the item directly
			Item = item;
			Position = GridDragPosition.Over;
			return true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.TreeGridDragInfo"/> class.
		/// </summary>
		/// <param name="control">The parent widget that this info belongs to</param>
		/// <param name="parent">Parent of the item dragging to.</param>
		/// <param name="item">Item user is dragging to, or null if dragging as a child of the parent node.</param>
		/// <param name="childIndex">Index of the item relative to the parent if known, otherwise null to determine the index when requsted.</param>
		/// <param name="position">The position of the cursor relative to the item or parent if item is null.</param>
		public TreeGridViewDragInfo(TreeGridView control, object parent, object item, int? childIndex, GridDragPosition position)
		{
			Control = control;
			_parent = parent;
			_childIndex = childIndex;
			_position = position;
			_item = item;
		}
	}

	/// <summary>
	/// Presents a tree with multiple columns
	/// </summary>
	[Handler(typeof(TreeGridView.IHandler))]
	public class TreeGridView : Grid
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Activated"/> event.
		/// </summary>
		public const string ActivatedEvent = "TreeGridView.ActivatedEvent";

		/// <summary>
		/// Occurs when the user activates an item by double clicking or pressing enter.
		/// </summary>
		public event EventHandler<TreeGridViewItemEventArgs> Activated
		{
			add { Properties.AddHandlerEvent(ActivatedEvent, value); }
			remove { Properties.RemoveEvent(ActivatedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnActivated(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Expanding"/> event.
		/// </summary>
		public const string ExpandingEvent = "TreeGridView.ExpandingEvent";

		/// <summary>
		/// Occurs before a tree item is expanded.
		/// </summary>
		public event EventHandler<TreeGridViewItemCancelEventArgs> Expanding
		{
			add { Properties.AddHandlerEvent(ExpandingEvent, value); }
			remove { Properties.RemoveEvent(ExpandingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Expanding"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnExpanding(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(ExpandingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Expanded"/> event.
		/// </summary>
		public const string ExpandedEvent = "TreeGridView.ExpandedEvent";

		/// <summary>
		/// Occurs after a tree item has been expanded.
		/// </summary>
		public event EventHandler<TreeGridViewItemEventArgs> Expanded
		{
			add { Properties.AddHandlerEvent(ExpandedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Expanded"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnExpanded(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ExpandedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Collapsing"/> event.
		/// </summary>
		public const string CollapsingEvent = "TreeGridView.CollapsingEvent";

		/// <summary>
		/// Occurs before a tree item is collapsed.
		/// </summary>
		public event EventHandler<TreeGridViewItemCancelEventArgs> Collapsing
		{
			add { Properties.AddHandlerEvent(CollapsingEvent, value); }
			remove { Properties.RemoveEvent(CollapsingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Collapsing"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCollapsing(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(CollapsingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Collapsed"/> event.
		/// </summary>
		public const string CollapsedEvent = "TreeGridView.CollapsedEvent";

		/// <summary>
		/// Occurs after a tree item is collapsed.
		/// </summary>
		public event EventHandler<TreeGridViewItemEventArgs> Collapsed
		{
			add { Properties.AddHandlerEvent(CollapsedEvent, value); }
			remove { Properties.RemoveEvent(CollapsedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Collapsed"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCollapsed(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(CollapsedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="SelectedItemChanged"/> event.
		/// </summary>
		public const string SelectedItemChangedEvent = "TreeGridView.SelectedItemChanged";

		/// <summary>
		/// Occurs when the <see cref="SelectedItem"/> has changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedItemChanged
		{
			add { Properties.AddHandlerEvent(SelectedItemChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectedItemChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedItemChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedItemChangedEvent, this, e);
		}

		#endregion

		static TreeGridView()
		{
			EventLookup.Register<TreeGridView>(c => c.OnExpanding(null), TreeGridView.ExpandingEvent);
			EventLookup.Register<TreeGridView>(c => c.OnExpanded(null), TreeGridView.ExpandedEvent);
			EventLookup.Register<TreeGridView>(c => c.OnCollapsing(null), TreeGridView.CollapsingEvent);
			EventLookup.Register<TreeGridView>(c => c.OnCollapsed(null), TreeGridView.CollapsedEvent);
			EventLookup.Register<TreeGridView>(c => c.OnSelectedItemChanged(null), TreeGridView.SelectedItemChangedEvent);
		}

		/// <summary>
		/// Gets or sets the selected item in the tree.
		/// </summary>
		/// <value>The selected item.</value>
		public new ITreeGridItem SelectedItem
		{
			get { return Handler.SelectedItem; }
			set { Handler.SelectedItem = value; }
		}

		/// <summary>
		/// Gets or sets the data store of tree items.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="TreeGridItemCollection"/> for easy creation of a tree.
		/// </remarks>
		/// <value>The data store.</value>
		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		/// <summary>
		/// Gets an enumeration of the currently selected items
		/// </summary>
		/// <value>The selected items.</value>
		public override IEnumerable<object> SelectedItems
		{
			get { return Handler.SelectedItems; }
		}

		/// <summary>
		/// Gets or sets the context menu when right clicking or pressing the menu key on an item.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		/// <summary>
		/// Refreshes the data, keeping the selection
		/// </summary>
		public void ReloadData()
		{
			Handler.ReloadData();
		}

		/// <summary>
		/// Refreshes the specified item and all its children, keeping the selection if not part of the refreshed nodes
		/// </summary>
		/// <param name="item">Item to refresh, including its children</param>
		public void ReloadItem(ITreeGridItem item) => ReloadItem(item, true);

		/// <summary>
		/// Refreshes the specified item and optionally all of its children, keeping the selection if not part of the refreshed nodes
		/// </summary>
		/// <param name="item">Item to refresh</param>
		/// <param name="reloadChildren">Reload children of the specified item</param>
		public void ReloadItem(ITreeGridItem item, bool reloadChildren) => Handler.ReloadItem(item, reloadChildren);

		/// <summary>
		/// Gets the node at a specified location from the origin of the control
		/// </summary>
		/// <remarks>
		/// Useful for determining which node is under the mouse cursor.
		/// </remarks>
		/// <returns>The item from the data store that is displayed at the specified location</returns>
		/// <param name="location">Point to find the node</param>
		public TreeGridCell GetCellAt(PointF location)
		{
			int column;
			var item = Handler.GetCellAt(location, out column);
			return new TreeGridCell(item, column >= 0 ? Columns[column] : null, column);
		}

		/// <summary>
		/// Gets the tree grid drag info for the specified DragEventArgs.
		/// </summary>
		/// <remarks>
		/// Use this to get or set information about where the drop will occur.
		/// </remarks>
		/// <returns>The drag information.</returns>
		/// <param name="args">Arguments to get the drag info for.</param>
		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => Handler.GetDragInfo(args);

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for the <see cref="TreeGridView"/>
		/// </summary>
		public new interface ICallback : Grid.ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			void OnActivated(TreeGridView widget, TreeGridViewItemEventArgs e);

			/// <summary>
			/// Raises the expanding event.
			/// </summary>
			void OnExpanding(TreeGridView widget, TreeGridViewItemCancelEventArgs e);

			/// <summary>
			/// Raises the expanded event.
			/// </summary>
			void OnExpanded(TreeGridView widget, TreeGridViewItemEventArgs e);

			/// <summary>
			/// Raises the collapsing event.
			/// </summary>
			void OnCollapsing(TreeGridView widget, TreeGridViewItemCancelEventArgs e);

			/// <summary>
			/// Raises the collapsed event.
			/// </summary>
			void OnCollapsed(TreeGridView widget, TreeGridViewItemEventArgs e);

			/// <summary>
			/// Raises the selected item changed event.
			/// </summary>
			void OnSelectedItemChanged(TreeGridView widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="TreeGridView"/>
		/// </summary>
		protected new class Callback : Grid.Callback, ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			public void OnActivated(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnActivated(e);
			}

			/// <summary>
			/// Raises the expanding event.
			/// </summary>
			public void OnExpanding(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnExpanding(e);
			}

			/// <summary>
			/// Raises the expanded event.
			/// </summary>
			public void OnExpanded(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnExpanded(e);
			}

			/// <summary>
			/// Raises the collapsing event.
			/// </summary>
			public void OnCollapsing(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCollapsing(e);
			}

			/// <summary>
			/// Raises the collapsed event.
			/// </summary>
			public void OnCollapsed(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCollapsed(e);
			}

			/// <summary>
			/// Raises the selected item changed event.
			/// </summary>
			public void OnSelectedItemChanged(TreeGridView widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedItemChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="TreeGridView"/>
		/// </summary>
		public new interface IHandler : Grid.IHandler, IContextMenuHost
		{
			/// <summary>
			/// Gets or sets the data store of tree items.
			/// </summary>
			/// <remarks>
			/// Use the <see cref="TreeGridItemCollection"/> for easy creation of a tree.
			/// </remarks>
			/// <value>The data store.</value>
			ITreeGridStore<ITreeGridItem> DataStore { get; set; }

			/// <summary>
			/// Gets or sets the selected item in the tree.
			/// </summary>
			/// <value>The selected item.</value>
			ITreeGridItem SelectedItem { get; set; }

			/// <summary>
			/// Gets an enumeration of the currently selected items
			/// </summary>
			/// <value>The selected items.</value>
			IEnumerable<object> SelectedItems { get; }

			/// <summary>
			/// Refreshes the data, keeping the selection
			/// </summary>
			void ReloadData();

			/// <summary>
			/// Refreshes the specified item and optionally all of its children, keeping the selection if not part of the refreshed nodes
			/// </summary>
			/// <param name="item">Item to refresh</param>
			/// <param name="reloadChildren">Reload children of the specified item</param>
			void ReloadItem(ITreeGridItem item, bool reloadChildren);

			/// <summary>
			/// Gets the item and column of a location in the control.
			/// </summary>
			/// <returns>The item from the data store that is displayed at the specified location</returns>
			/// <param name="location">Point to find the node</param>
			/// <param name="column">Column at the location, or -1 if no column (e.g. at the end of the row)</param>
			ITreeGridItem GetCellAt(PointF location, out int column);

			/// <summary>
			/// Gets the tree grid drag info for the specified DragEventArgs.
			/// </summary>
			/// <remarks>
			/// Use this to get or set information about where the drop will occur.
			/// </remarks>
			/// <returns>The drag information.</returns>
			/// <param name="args">Arguments to get the drag info for.</param>
			TreeGridViewDragInfo GetDragInfo(DragEventArgs args);
		}
	}
}
