using System;
using SD = System.Drawing;
using Eto.Forms;
using System.Linq;
using MonoMac.AppKit;
using System.Collections.Generic;
using System.Collections;

namespace Eto.Mac.Forms.Controls
{
	public class ComboBoxHandler : MacControl<NSPopUpButton, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
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
			handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				var binding = Handler.Widget.TextBinding;
				return Handler.Control.Menu.IndexOf(binding.GetValue(item));
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				foreach (var item in items.Select(r => binding.GetValue(r)))
				{
					Handler.Control.Menu.AddItem(new NSMenuItem(item));
				}
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Menu.AddItem(new NSMenuItem(Convert.ToString(binding.GetValue(item))));
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Menu.InsertItem(new NSMenuItem(Convert.ToString(binding.GetValue(item))), index);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveItem(int index)
			{
				var selected = Handler.SelectedIndex;
				Handler.Control.RemoveItem(index);
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && selected == index)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			public override void RemoveAllItems()
			{
				var change = Handler.SelectedIndex != -1;
				Handler.Control.RemoveAllItems();
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && change)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection == null ? null : collection.Collection; }
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
						Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}
	}
}
