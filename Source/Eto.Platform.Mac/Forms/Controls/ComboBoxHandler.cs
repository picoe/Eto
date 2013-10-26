using System;
using SD = System.Drawing;
using Eto.Forms;
using System.Linq;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ComboBoxHandler : MacControl<NSPopUpButton, ComboBox>, IComboBox
	{
		CollectionHandler collection;

		public class EtoPopUpButton : NSPopUpButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public ComboBoxHandler()
		{
			Control = new EtoPopUpButton { Handler = this };
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ComboBoxHandler;
			handler.Widget.OnSelectedIndexChanged(EventArgs.Empty);
		}

		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ComboBoxHandler Handler { get; set; }

			public override int IndexOf(IListItem item)
			{
				return Handler.Control.IndexOfItem(item.Text);
			}

			public override void AddRange(IEnumerable<IListItem> items)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.AddItems(items.Select(r => r.Text).ToArray());
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(IListItem item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.AddItem(item.Text);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, IListItem item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.InsertItem(item.Text, index);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.RemoveItem(index);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveAllItems()
			{
				Handler.Control.RemoveAllItems();
				Handler.LayoutIfNeeded();
			}
		}

		public IListStore DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public int SelectedIndex
		{
			get	{ return Control.IndexOfSelectedItem; }
			set
			{
				if (value != SelectedIndex)
				{
					Control.SelectItem(value);
					if (Widget.Loaded)
						Widget.OnSelectedIndexChanged(EventArgs.Empty);
				}
			}
		}
	}
}
