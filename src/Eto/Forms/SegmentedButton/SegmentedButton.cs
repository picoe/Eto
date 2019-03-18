using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the selection modes for the <see cref="SegmentedButton"/>.
	/// </summary>
	public enum SegmentedSelectionMode
	{
		/// <summary>
		/// No selection is possible, but you can click on each segment to trigger its click event.
		/// </summary>
		None,
		/// <summary>
		/// Only a single segment can be selected at a time
		/// </summary>
		/// <remarks>
		/// When clicking on a selected segment in this mode it will remain selected.
		/// </remarks>
		Single,
		/// <summary>
		/// Multiple segments can be selected
		/// </summary>
		/// <remarks>
		/// Clicking a segment will toggle its selection on or off.
		/// </remarks>
		Multiple
	}

	/// <summary>
	/// Button with multiple segments that can be clicked.
	/// </summary>
	/// <remarks>
	/// The SegmentedButton allows you to group multiple buttons together visually.
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class SegmentedButton : Control, IBindableWidgetContainer
	{
		new IHandler Handler => (IHandler)base.Handler;

		static SegmentedButton()
		{
			EventLookup.Register<SegmentedButton>(c => c.OnSelectedItemsChanged(null), SelectedIndexesChangedEvent);
			EventLookup.Register<SegmentedButton>(c => c.OnSelectedItemChanged(null), SelectedIndexesChangedEvent);
			EventLookup.Register<SegmentedButton>(c => c.OnSelectedIndexChanged(null), SelectedIndexesChangedEvent);
			EventLookup.Register<SegmentedButton>(c => c.OnSelectedIndexesChanged(null), SelectedIndexesChangedEvent);
			EventLookup.Register<SegmentedButton>(c => c.OnItemClicked(null), ItemClickEvent);
		}

		/// <summary>
		/// Gets the collection of segmented items
		/// </summary>
		/// <value>The segmented items.</value>
		public SegmentedItemCollection Items { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.SegmentedButton"/> class.
		/// </summary>
		public SegmentedButton()
		{
			Items = new SegmentedItemCollection(this);
		}

		/// <inheritdoc />
		protected override void OnApplyCascadingStyles()
		{
			base.OnApplyCascadingStyles();
			foreach (var item in Items)
				ApplyStyles(item, item.Style);
		}

		/// <summary>
		/// Gets or sets the selection mode.
		/// </summary>
		/// <value>The selection mode.</value>
		public SegmentedSelectionMode SelectionMode
		{
			get => Handler.SelectionMode;
			set => Handler.SelectionMode = value;
		}

		#region Events


		static readonly object SelectedItemsChangedEvent = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedItems"/> have changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedItemsChanged
		{
			add => Properties.AddEvent(SelectedItemsChangedEvent, value);
			remove => Properties.RemoveEvent(SelectedItemsChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedItemsChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSelectedItemsChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedItemsChangedEvent, this, e);
			OnSelectedItemChanged(e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="SelectedIndexesChanged"/> event.
		/// </summary>
		public const string SelectedIndexesChangedEvent = "SegmentedButton.SelectedIndexesChanged";

		/// <summary>
		/// Occurs when the <see cref="SelectedIndexes"/> have changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexesChanged
		{
			add => Properties.AddHandlerEvent(SelectedIndexesChangedEvent, value);
			remove => Properties.RemoveEvent(SelectedIndexesChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexesChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSelectedIndexesChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedIndexesChangedEvent, this, e);
			OnSelectedIndexChanged(e);
			OnSelectedItemsChanged(e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="ItemClick"/> event.
		/// </summary>
		public const string ItemClickEvent = "SegmentedButton.ItemClick";

		/// <summary>
		/// Occurs when an item has been clicked.
		/// </summary>
		/// <seealso cref="SegmentedItem.Click"/>
		public event EventHandler<SegmentedItemClickEventArgs> ItemClick
		{
			add => Properties.AddHandlerEvent(ItemClickEvent, value);
			remove => Properties.RemoveEvent(ItemClickEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="ItemClick"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnItemClicked(SegmentedItemClickEventArgs e)
		{
			Properties.TriggerEvent(ItemClickEvent, this, e);
		}

		static readonly object SelectedIndexChangedEvent = new object();

		/// <summary>
		/// Event to handle when the <see cref="SelectedIndex"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexChanged
		{
			add => Properties.AddEvent(SelectedIndexChangedEvent, value);
			remove => Properties.RemoveEvent(SelectedIndexChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedIndexChangedEvent, this, e);
		}

		static readonly object SelectedItemChangedEvent = new object();

		/// <summary>
		/// Event to handle when the <see cref="SelectedItem"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedItemChanged
		{
			add => Properties.AddEvent(SelectedItemChangedEvent, value);
			remove => Properties.RemoveEvent(SelectedItemChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedItemChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedItemChangedEvent, this, e);
		}

		#endregion

		/// <summary>
		/// Gets or sets the selected items.
		/// </summary>
		/// <remarks>
		/// You can only set selected item based on the current value of <see cref="SelectionMode"/>.
		/// 
		/// For <see cref="SegmentedSelectionMode.Multiple"/>, any combination of items can be selected.
		/// For <see cref="SegmentedSelectionMode.Single"/>, only a single item will be selected. Setting multiple items in this mode
		/// will only result in a single item being selected.
		/// For <see cref="SegmentedSelectionMode.None"/>, no items can be selected.
		/// </remarks>
		/// <value>The selected items.</value>
		public IEnumerable<SegmentedItem> SelectedItems
		{
			get => SelectedIndexes?.Select(r => Items[r]) ?? Enumerable.Empty<SegmentedItem>();
			set => SelectedIndexes = value?.Select(r => Items.IndexOf(r));
		}

		/// <summary>
		/// Gets or sets the selected item, or null for no selection.
		/// </summary>
		/// <remarks>
		/// This works when <see cref="SelectionMode"/> is either single or multiple.
		/// 
		/// When setting this value in multiple selection mode, all other selected items will be cleared.
		/// </remarks>
		/// <value>The selected item, <c>null</c> when nothing is selected.</value>
		public SegmentedItem SelectedItem
		{
			get
			{
				var index = SelectedIndex;
				if (index == -1)
					return null;
				return Items[index];
			}
			set => SelectedIndex = Items.IndexOf(value);
		}

		/// <summary>
		/// Gets or sets the selected indexes.
		/// </summary>
		/// <remarks>
		/// You can only set selected indexes based on the current value of <see cref="SelectionMode"/>.
		/// 
		/// For <see cref="SegmentedSelectionMode.Multiple"/>, any combination of indexes can be selected.
		/// For <see cref="SegmentedSelectionMode.Single"/>, only a single index will be selected. Setting multiple indexes in this mode
		/// will only result in a single index being selected.
		/// For <see cref="SegmentedSelectionMode.None"/>, no indexes can be selected.
		/// </remarks>
		/// <value>The selected indexes.</value>
		public IEnumerable<int> SelectedIndexes
		{
			get => Handler.SelectedIndexes;
			set => Handler.SelectedIndexes = value;
		}

		/// <summary>
		/// Gets or sets the index of the selected item, or -1 for no selection.
		/// </summary>
		/// <remarks>
		/// This works when <see cref="SelectionMode"/> is either single or multiple.
		/// 
		/// When setting this value in multiple selection mode, all other selected items will be cleared.
		/// </remarks>
		/// <value>The index of the selected item, or -1 when nothing is selected.</value>
		public int SelectedIndex
		{
			get => Handler.SelectedIndex;
			set => Handler.SelectedIndex = value;
		}

		IEnumerable<BindableWidget> IBindableWidgetContainer.Children => Items;

		/// <summary>
		/// Selects all items when <see cref="SelectionMode"/> is set to Multiple.
		/// </summary>
		public void SelectAll() => Handler.SelectAll();

		/// <summary>
		/// Clears all selected items.
		/// </summary>
		public void ClearSelection() => Handler.ClearSelection();

		static readonly Callback s_callback = new Callback();

		/// <inheritdoc />
		protected override object GetCallback() => s_callback;

		/// <summary>
		/// Callback methods for handlers of <see cref="SegmentedButton"/>.
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the ItemClicked event.
			/// </summary>
			public void OnItemClicked(SegmentedButton widget, SegmentedItemClickEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnItemClicked(e);
			}

			/// <summary>
			/// Raises the SelectedIndexesChanged event
			/// </summary>
			public void OnSelectedIndexesChanged(SegmentedButton widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedIndexesChanged(e);
			}
		}

		/// <summary>
		/// Callback interface for handlers of <see cref="SegmentedButton"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the ItemClicked event.
			/// </summary>
			void OnItemClicked(SegmentedButton widget, SegmentedItemClickEventArgs e);

			/// <summary>
			/// Raises the SelectedIndexesChanged event
			/// </summary>
			void OnSelectedIndexesChanged(SegmentedButton widget, EventArgs e);
		}

		/// <summary>
		/// Handler interface for <see cref="SegmentedButton"/>.
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the index of the selected item, or -1 for no selection.
			/// </summary>
			/// <remarks>
			/// This works when <see cref="SelectionMode"/> is either single or multiple.
			/// 
			/// When setting this value in multiple selection mode, all other selected items will be cleared.
			/// </remarks>
			/// <value>The index of the selected item, or -1 when nothing is selected.</value>
			int SelectedIndex { get; set; }

			/// <summary>
			/// Gets or sets the selected indexes.
			/// </summary>
			/// <remarks>
			/// You can only set selected indexes based on the current value of <see cref="SelectionMode"/>.
			/// 
			/// For <see cref="SegmentedSelectionMode.Multiple"/>, any combination of indexes can be selected.
			/// For <see cref="SegmentedSelectionMode.Single"/>, only a single index will be selected. Setting multiple indexes in this mode
			/// will only result in a single index being selected.
			/// For <see cref="SegmentedSelectionMode.None"/>, no indexes can be selected.
			/// </remarks>
			/// <value>The selected indexes.</value>
			IEnumerable<int> SelectedIndexes { get; set; }

			/// <summary>
			/// Gets or sets the selection mode.
			/// </summary>
			/// <value>The selection mode.</value>
			SegmentedSelectionMode SelectionMode { get; set; }

			/// <summary>
			/// Selects all items when <see cref="SelectionMode"/> is set to Multiple.
			/// </summary>
			void SelectAll();

			/// <summary>
			/// Clears all selected items.
			/// </summary>
			void ClearSelection();

			/// <summary>
			/// Clears all items from the segmented button.
			/// </summary>
			void ClearItems();

			/// <summary>
			/// Inserts the item at the specified index.
			/// </summary>
			/// <param name="index">Index to insert at.</param>
			/// <param name="item">Item to insert.</param>
			void InsertItem(int index, SegmentedItem item);

			/// <summary>
			/// Removes the item at the specified index.
			/// </summary>
			/// <param name="index">Index to remove.</param>
			/// <param name="item">Item that is being removed.</param>
			void RemoveItem(int index, SegmentedItem item);

			/// <summary>
			/// Sets the item at the specified index, replacing its existing item.
			/// </summary>
			/// <param name="index">Index to replace at.</param>
			/// <param name="item">Item to replace with.</param>
			void SetItem(int index, SegmentedItem item);
		}
	}
}
