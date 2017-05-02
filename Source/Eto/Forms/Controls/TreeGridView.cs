using System;
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
	/// Presents a tree with multiple columns
	/// </summary>
	[Handler(typeof(TreeGridView.IHandler))]
	public class TreeGridView : Grid
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Occurs when the user activates an item by double clicking or pressing enter.
		/// </summary>
		public event EventHandler<TreeGridViewItemEventArgs> Activated
		{
			add { Properties.AddEvent(ActivatedKey, value); }
			remove { Properties.RemoveEvent(ActivatedKey, value); }
		}

		static readonly object ActivatedKey = new object();

		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnActivated(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedKey, this, e);
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
		/// <param name="item">Item to refresh</param>
		public void ReloadItem(ITreeGridItem item)
		{
			Handler.ReloadItem(item);
		}

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
				widget.Platform.Invoke(() => widget.OnActivated(e));
			}

			/// <summary>
			/// Raises the expanding event.
			/// </summary>
			public void OnExpanding(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnExpanding(e));
			}

			/// <summary>
			/// Raises the expanded event.
			/// </summary>
			public void OnExpanded(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnExpanded(e));
			}

			/// <summary>
			/// Raises the collapsing event.
			/// </summary>
			public void OnCollapsing(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnCollapsing(e));
			}

			/// <summary>
			/// Raises the collapsed event.
			/// </summary>
			public void OnCollapsed(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnCollapsed(e));
			}

			/// <summary>
			/// Raises the selected item changed event.
			/// </summary>
			public void OnSelectedItemChanged(TreeGridView widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnSelectedItemChanged(e));
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
			/// Refreshes the specified item and all its children, keeping the selection if not part of the refreshed nodes
			/// </summary>
			/// <param name="item">Item to refresh</param>
			void ReloadItem(ITreeGridItem item);

			/// <summary>
			/// Gets the item and column of a location in the control.
			/// </summary>
			/// <returns>The item from the data store that is displayed at the specified location</returns>
			/// <param name="location">Point to find the node</param>
			/// <param name="column">Column at the location, or -1 if no column (e.g. at the end of the row)</param>
			ITreeGridItem GetCellAt(PointF location, out int column);
		}
	}
}