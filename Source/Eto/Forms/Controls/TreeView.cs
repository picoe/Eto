using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base data store for the <see cref="TreeView"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ITreeStore : IDataStore<ITreeItem>
	{
	}

	/// <summary>
	/// Event arguments for <see cref="TreeView"/> events relating to an item
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TreeViewItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the item that triggered the event
		/// </summary>
		/// <value>The item.</value>
		public ITreeItem Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeViewItemEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item.</param>
		public TreeViewItemEventArgs(ITreeItem item)
		{
			this.Item = item;
		}
	}

	/// <summary>
	/// Event arguments for <see cref="TreeView"/> events that can be cancelled
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TreeViewItemCancelEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Gets the item that triggered the event
		/// </summary>
		/// <value>The item.</value>
		public ITreeItem Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeViewItemCancelEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item.</param>
		public TreeViewItemCancelEventArgs(ITreeItem item)
		{
			this.Item = item;
		}
	}

	/// <summary>
	/// Event arguments for <see cref="TreeView"/> events that can modify the label of the text
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TreeViewItemEditEventArgs : TreeViewItemCancelEventArgs
	{
		/// <summary>
		/// Gets or sets the label of the item
		/// </summary>
		/// <value>The label.</value>
		public string Label { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeViewItemEditEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="label">Label.</param>
		public TreeViewItemEditEventArgs(ITreeItem item, string label)
			: base(item)
		{
			this.Label = label;
		}
	}

	/// <summary>
	/// Standard tree view control with a single column
	/// </summary>
	/// <remarks>
	/// This uses the standard tree view controls on windows, so it can sometimes be more desirable to use to give a more
	/// natural feel in that case.
	/// 
	/// For a tree with multiple columns, use the <see cref="TreeGridView"/>.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(TreeView.IHandler))]
	public class TreeView : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Activated"/> event
		/// </summary>
		public const string ActivatedEvent = "TreeView.Activated";

		/// <summary>
		/// Event to handle when an item is activated
		/// </summary>
		/// <remarks>
		/// An item is activated typically when double clicking the mouse on the item, or pressing enter with it selected.
		/// </remarks>
		public event EventHandler<TreeViewItemEventArgs> Activated
		{
			add { Properties.AddHandlerEvent(ActivatedEvent, value); }
			remove { Properties.RemoveEvent(ActivatedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnActivated(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="SelectionChanged"/> event
		/// </summary>
		public const string SelectionChangedEvent = "TreeView.SelectionChangedEvent";

		/// <summary>
		/// Occurs when the selection has been changed by the user or programattically
		/// </summary>
		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Expanding"/> event
		/// </summary>
		public const string ExpandingEvent = "TreeView.ExpandingEvent";

		/// <summary>
		/// Occurs before an item is expanding
		/// </summary>
		public event EventHandler<TreeViewItemCancelEventArgs> Expanding
		{
			add { Properties.AddHandlerEvent(ExpandingEvent, value); }
			remove { Properties.RemoveEvent(ExpandingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Expanding"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnExpanding(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(ExpandingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Expanded"/> event
		/// </summary>
		public const string ExpandedEvent = "TreeView.ExpandedEvent";

		/// <summary>
		/// Occurs after an item has been expanded
		/// </summary>
		public event EventHandler<TreeViewItemEventArgs> Expanded
		{
			add { Properties.AddHandlerEvent(ExpandedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Expanded"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnExpanded(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(ExpandedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Collapsing"/> event
		/// </summary>
		public const string CollapsingEvent = "TreeView.CollapsingEvent";

		/// <summary>
		/// Occurs before an item is collapsed
		/// </summary>
		public event EventHandler<TreeViewItemCancelEventArgs> Collapsing
		{
			add { Properties.AddHandlerEvent(CollapsingEvent, value); }
			remove { Properties.RemoveEvent(CollapsingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Collapsing"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCollapsing(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(CollapsingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Collapsed"/> event
		/// </summary>
		public const string CollapsedEvent = "TreeView.CollapsedEvent";

		/// <summary>
		/// Occurs after an item has been collapsed
		/// </summary>
		public event EventHandler<TreeViewItemEventArgs> Collapsed
		{
			add { Properties.AddHandlerEvent(CollapsedEvent, value); }
			remove { Properties.RemoveEvent(CollapsedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Collapsed"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCollapsed(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(CollapsedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="LabelEdited"/> event
		/// </summary>
		public const string LabelEditedEvent = "TreeView.LabelEdited";

		/// <summary>
		/// Occurs after the label of an item has been edited
		/// </summary>
		public event EventHandler<TreeViewItemEditEventArgs> LabelEdited
		{
			add { Properties.AddHandlerEvent(LabelEditedEvent, value); }
			remove { Properties.RemoveEvent(LabelEditedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="LabelEdited"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLabelEdited(TreeViewItemEditEventArgs e)
		{
			Properties.TriggerEvent(LabelEditedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="LabelEditing"/> event
		/// </summary>
		public const string LabelEditingEvent = "TreeView.LabelEditing";

		/// <summary>
		/// Occurs before an items label is edited
		/// </summary>
		public event EventHandler<TreeViewItemCancelEventArgs> LabelEditing
		{
			add { Properties.AddHandlerEvent(LabelEditingEvent, value); }
			remove { Properties.RemoveEvent(LabelEditingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="LabelEditing"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLabelEditing(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(LabelEditingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="NodeMouseClick"/> event
		/// </summary>
		public const string NodeMouseClickEvent = "TreeView.NodeMouseClick";

		/// <summary>
		/// Occurs when a node is clicked with the mouse
		/// </summary>
		public event EventHandler<TreeViewItemEventArgs> NodeMouseClick
		{
			add { Properties.AddHandlerEvent(NodeMouseClickEvent, value); }
			remove { Properties.RemoveEvent(NodeMouseClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="NodeMouseClick"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnNodeMouseClick(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(NodeMouseClickEvent, this, e);
		}

		#endregion

		static TreeView()
		{
			EventLookup.Register<TreeView>(c => c.OnActivated(null), TreeView.ActivatedEvent);
			EventLookup.Register<TreeView>(c => c.OnSelectionChanged(null), TreeView.SelectionChangedEvent);
			EventLookup.Register<TreeView>(c => c.OnExpanding(null), TreeView.ExpandingEvent);
			EventLookup.Register<TreeView>(c => c.OnExpanded(null), TreeView.ExpandedEvent);
			EventLookup.Register<TreeView>(c => c.OnCollapsing(null), TreeView.CollapsingEvent);
			EventLookup.Register<TreeView>(c => c.OnCollapsed(null), TreeView.CollapsedEvent);
			EventLookup.Register<TreeView>(c => c.OnLabelEdited(null), TreeView.LabelEditedEvent);
			EventLookup.Register<TreeView>(c => c.OnLabelEditing(null), TreeView.LabelEditingEvent);
			EventLookup.Register<TreeView>(c => c.OnNodeMouseClick(null), TreeView.NodeMouseClickEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeView"/> class.
		/// </summary>
		public TreeView()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public TreeView(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TreeView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		public ITreeItem SelectedItem
		{
			get { return Handler.SelectedItem; }
			set { Handler.SelectedItem = value; }
		}

		/// <summary>
		/// Gets or sets the data store.
		/// </summary>
		/// <value>The data store.</value>
		public ITreeStore DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		/// <summary>
		/// Gets or sets the color of the text for all nodes.
		/// </summary>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		/// <summary>
		/// Refreshes the data, keeping the selection
		/// </summary>
		public void RefreshData()
		{
			Handler.RefreshData();
		}

		/// <summary>
		/// Refreshes the specified item and all its children, keeping the selection if not part of the refreshed nodes
		/// </summary>
		/// <param name="item">Item to refresh</param>
		public void RefreshItem(ITreeItem item)
		{
			Handler.RefreshItem(item);
		}

		/// <summary>
		/// Gets the node at a specified point from the origin of the control
		/// </summary>
		/// <remarks>
		/// Useful for determining which node is under the mouse cursor.
		/// </remarks>
		/// <returns>The item from the data store that is displayed at the specified location</returns>
		/// <param name="point">Point to find the node</param>
		public ITreeItem GetNodeAt(PointF point)
		{
			return Handler.GetNodeAt(point);
		}

		/// <summary>
		/// Gets or sets a value indicating whether users can edit the labels of items
		/// </summary>
		/// <seealso cref="LabelEditing"/>
		/// <seealso cref="LabelEdited"/>
		/// <value><c>true</c> to allow label editing; otherwise, <c>false</c>.</value>
		public bool LabelEdit
		{
			get { return Handler.LabelEdit; }
			set { Handler.LabelEdit = value; }
		}

		/// <summary>
		/// Gets or sets the context menu to show when the user right clicks or presses the menu key
		/// </summary>
		/// <value>The context menu to show, or null to have no menu</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for instances of <see cref="TreeView"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			void OnActivated(TreeView widget, TreeViewItemEventArgs e);
			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			void OnSelectionChanged(TreeView widget, EventArgs e);
			/// <summary>
			/// Raises the expanding event.
			/// </summary>
			void OnExpanding(TreeView widget, TreeViewItemCancelEventArgs e);
			/// <summary>
			/// Raises the expanded event.
			/// </summary>
			void OnExpanded(TreeView widget, TreeViewItemEventArgs e);
			/// <summary>
			/// Raises the collapsing event.
			/// </summary>
			void OnCollapsing(TreeView widget, TreeViewItemCancelEventArgs e);
			/// <summary>
			/// Raises the collapsed event.
			/// </summary>
			void OnCollapsed(TreeView widget, TreeViewItemEventArgs e);
			/// <summary>
			/// Raises the label edited event.
			/// </summary>
			void OnLabelEdited(TreeView widget, TreeViewItemEditEventArgs e);
			/// <summary>
			/// Raises the label editing event.
			/// </summary>
			void OnLabelEditing(TreeView widget, TreeViewItemCancelEventArgs e);
			/// <summary>
			/// Raises the node mouse click event.
			/// </summary>
			void OnNodeMouseClick(TreeView widget, TreeViewItemEventArgs e);
		}

		/// <summary>
		/// Callback methods for handlers of <see cref="TreeView"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			public void OnActivated(TreeView widget, TreeViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnActivated(e));
			}
			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			public void OnSelectionChanged(TreeView widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnSelectionChanged(e));
			}
			/// <summary>
			/// Raises the expanding event.
			/// </summary>
			public void OnExpanding(TreeView widget, TreeViewItemCancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnExpanding(e));
			}
			/// <summary>
			/// Raises the expanded event.
			/// </summary>
			public void OnExpanded(TreeView widget, TreeViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnExpanded(e));
			}
			/// <summary>
			/// Raises the collapsing event.
			/// </summary>
			public void OnCollapsing(TreeView widget, TreeViewItemCancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnCollapsing(e));
			}
			/// <summary>
			/// Raises the collapsed event.
			/// </summary>
			public void OnCollapsed(TreeView widget, TreeViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnCollapsed(e));
			}
			/// <summary>
			/// Raises the label edited event.
			/// </summary>
			public void OnLabelEdited(TreeView widget, TreeViewItemEditEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnLabelEdited(e));
			}
			/// <summary>
			/// Raises the label editing event.
			/// </summary>
			public void OnLabelEditing(TreeView widget, TreeViewItemCancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnLabelEditing(e));
			}
			/// <summary>
			/// Raises the node mouse click event.
			/// </summary>
			public void OnNodeMouseClick(TreeView widget, TreeViewItemEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnNodeMouseClick(e));
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for <see cref="TreeView"/>
		/// </summary>
		public new interface IHandler : Control.IHandler, IContextMenuHost
		{
			/// <summary>
			/// Gets or sets the data store.
			/// </summary>
			/// <value>The data store.</value>
			ITreeStore DataStore { get; set; }

			/// <summary>
			/// Gets or sets the selected item.
			/// </summary>
			/// <value>The selected item.</value>
			ITreeItem SelectedItem { get; set; }

			/// <summary>
			/// Refreshes the data, keeping the selection
			/// </summary>
			void RefreshData();

			/// <summary>
			/// Refreshes the specified item and all its children, keeping the selection if not part of the refreshed nodes
			/// </summary>
			/// <param name="item">Item to refresh</param>
			void RefreshItem(ITreeItem item);

			/// <summary>
			/// Gets the node at a specified point from the origin of the control
			/// </summary>
			/// <remarks>
			/// Useful for determining which node is under the mouse cursor.
			/// </remarks>
			/// <returns>The item from the data store that is displayed at the specified location</returns>
			/// <param name="point">Point to find the node</param>
			ITreeItem GetNodeAt(PointF point);

			/// <summary>
			/// Gets or sets a value indicating whether users can edit the labels of items
			/// </summary>
			/// <value><c>true</c> to allow label editing; otherwise, <c>false</c>.</value>
			bool LabelEdit { get; set; }

			/// <summary>
			/// Gets or sets the color of the text for all nodes.
			/// </summary>
			/// <value>The color of the text.</value>
			Color TextColor { get; set; }
		}

		#endregion
	}
}