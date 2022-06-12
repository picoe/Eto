using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// A collection of <see cref="ListItem"/> objects for use with <see cref="ListControl"/> objects
	/// </summary>
	/// <remarks>
	/// This is used to provide an easy way to add items to a <see cref="ListControl"/>.
	/// It is not mandatory to use this collection, however, since each control can specify bindings to your own
	/// model objects using <see cref="ListControl.ItemKeyBinding"/>, <see cref="ListControl.ItemTextBinding"/>, or other
	/// subclass bindings.
	/// </remarks>
	public class ListItemCollection : ExtendedObservableCollection<IListItem>
	{
		/// <summary>
		/// Initializes a new instance of the ListItemCollection class.
		/// </summary>
		public ListItemCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ListItemCollection class with the specified collection.
		/// </summary>
		/// <param name="collection">Collection of items to populate this collection with</param>
		public ListItemCollection(IEnumerable<IListItem> collection)
			: base(collection)
		{
		}

		/// <summary>
		/// Adds a new item to the list with the specified text
		/// </summary>
		/// <param name="text">Text to display for the item.</param>
		public void Add(string text)
		{
			base.Add(new ListItem{ Text = text });
		}

		/// <summary>
		/// Add a new item to the list with the specified text and key
		/// </summary>
		/// <param name="text">Text to display for the item.</param>
		/// <param name="key">Key for the item.</param>
		public void Add(string text, string key)
		{
			base.Add(new ListItem { Text = text, Key = key });
		}
	}

	class ListItemTextBinding : PropertyBinding<string>
	{
		public ListItemTextBinding()
			: base("Text")
		{
		}

		protected override string InternalGetValue(object dataItem)
		{
			var item = dataItem as IListItem;
			if (item != null)
				return item.Text;
			if (HasProperty(dataItem))
				return base.InternalGetValue(dataItem);
			return dataItem != null ? System.Convert.ToString(dataItem) : null;
		}
		protected override void InternalSetValue(object dataItem, string value)
		{
			var item = dataItem as IListItem;
			if (item != null)
				item.Text = System.Convert.ToString(value);
			else
				base.InternalSetValue(dataItem, value);
		}
	}

	class ListItemImageBinding : PropertyBinding<Image>
	{
		public ListItemImageBinding()
			: base("Image")
		{
		}

		protected override Image InternalGetValue(object dataItem)
		{
			var item = dataItem as IImageListItem;
			if (item != null)
				return item.Image;
			return base.InternalGetValue(dataItem);
		}
	}

	class ListItemKeyBinding : PropertyBinding<string>
	{
		public ListItemKeyBinding()
			: base("Key")
		{
		}

		protected override string InternalGetValue(object dataItem)
		{
			var item = dataItem as IListItem;
			if (item != null)
				return item.Key;
			if (HasProperty(dataItem))
				return base.InternalGetValue(dataItem);
			return dataItem != null ? System.Convert.ToString(dataItem) : null;
		}
	}

	/// <summary>
	/// Base control binding to a list of items
	/// </summary>
	[ContentProperty("Items")]
	public abstract class ListControl : CommonControl
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets the binding for the text value of each item.
		/// </summary>
		/// <value>The text binding.</value>
		public IIndirectBinding<string> ItemTextBinding
		{
			get => Handler.ItemTextBinding;
			set => Handler.ItemTextBinding = value;
		}

		/// <summary>
		/// Gets or sets the binding for the key value of each item.
		/// </summary>
		/// <value>The key binding.</value>
		public IIndirectBinding<string> ItemKeyBinding
		{
			get => Handler.ItemKeyBinding;
			set => Handler.ItemKeyBinding = value;
		}

		/// <summary>
		/// Gets or sets the binding for the text value of each item.
		/// </summary>
		/// <value>The text binding.</value>
		[Obsolete("Since 2.1: Use ItemTextBinding instead")]
		public IIndirectBinding<string> TextBinding
		{
			get { return ItemTextBinding; }
			set { ItemTextBinding = value; }
		}

		/// <summary>
		/// Gets or sets the binding for the key value of each item.
		/// </summary>
		/// <value>The key binding.</value>
		[Obsolete("Since 2.1: Use ItemKeyBinding instead")]
		public IIndirectBinding<string> KeyBinding
		{
			get { return ItemKeyBinding; }
			set { ItemKeyBinding = value; }
		}

		static readonly object SelectedIndexChangedKey = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedIndex"/> changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexChanged
		{
			add { Properties.AddEvent(SelectedIndexChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedIndexChangedKey, value); }
		}
		object lastSelectedValue;
		string lastSelectedKey;

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedIndexChangedKey, this, e);
			var newSelectedValue = SelectedValue;
			if (lastSelectedValue != newSelectedValue)
			{
				lastSelectedValue = newSelectedValue;
				OnSelectedValueChanged(e);
			}

			var newSelectedKey = SelectedKey;
			if (lastSelectedKey != newSelectedKey)
			{
				lastSelectedKey = newSelectedKey;
				OnSelectedKeyChanged(e);
			}

		}

		static readonly object SelectedValueChangedKey = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedValue"/> changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedValueChanged
		{
			add { Properties.AddEvent(SelectedValueChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedValueChangedKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedValueChangedKey, this, e);
		}

		static readonly object SelectedKeyChangedKey = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedValue"/> changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedKeyChanged
		{
			add { Properties.AddEvent(SelectedKeyChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedKeyChangedKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedKeyChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedKeyChangedKey, this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ListControl"/> class.
		/// </summary>
		protected ListControl()
		{
			ItemTextBinding = new ListItemTextBinding();
			ItemKeyBinding = new ListItemKeyBinding();
		}

		/// <summary>
		/// Gets the list of items in the control.
		/// </summary>
		/// <remarks>
		/// This is an alternate to using <see cref="DataStore"/> to easily add items to the list, when you do not
		/// want to use custom objects as the source for the list.
		/// This will set the <see cref="DataStore"/> to a new instance of a <see cref="ListItemCollection"/>.
		/// </remarks>
		/// <value>The items.</value>
		public ListItemCollection Items
		{
			get
			{
				var items = DataStore as ListItemCollection;
				if (items == null)
				{
					items = (ListItemCollection)CreateDefaultDataStore();
					DataStore = items;
				}
				return items;
			}
		}

		/// <summary>
		/// Gets or sets the data store for the items of the list control.
		/// </summary>
		/// <value>The data store.</value>
		public IEnumerable<object> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		/// <summary>
		/// Gets or sets the index of the currently selected item in the <see cref="DataStore"/>
		/// </summary>
		/// <value>The index of the selected item.</value>
		public int SelectedIndex
		{
			get { return Handler.SelectedIndex; }
			set { Handler.SelectedIndex = value; }
		}

		/// <summary>
		/// Gets or sets the selected object value of the item in <see cref="DataStore"/>
		/// </summary>
		/// <value>The selected value.</value>
		public object SelectedValue
		{
			get { return (SelectedIndex >= 0 && Handler.DataStore != null) ? Handler.DataStore.StoreElementAt<object>(SelectedIndex) : null; }
			set
			{
				EnsureDataStore();
				SelectedIndex = Handler.DataStore != null ? Handler.DataStore.IndexOf(value) : -1;
			}
		}

		/// <summary>
		/// Gets or sets the key of the selected item in the <see cref="DataStore"/>.
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="ItemKeyBinding"/> to map the key for each item in the list.
		/// </remarks>
		/// <value>The selected key.</value>
		public string SelectedKey
		{
			get { return ItemKeyBinding.GetValue(SelectedValue); }
			set
			{
				EnsureDataStore();
				SelectedIndex = Handler.DataStore != null ? Handler.DataStore.FindIndex<object>(r => ItemKeyBinding.GetValue(r) == value) : -1;
			}
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <remarks>
		/// By default, the text will get a color based on the user's theme. However, this is usually black.
		/// </remarks>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		/// <summary>
		/// Raises the <see cref="Eto.Forms.Control.LoadComplete"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			EnsureDataStore();
		}

		internal void EnsureDataStore()
		{
			if (DataStore == null)
				DataStore = CreateDefaultDataStore();
		}

		/// <summary>
		/// Creates the default data store for the list.
		/// </summary>
		/// <remarks>
		/// This is used to create a data store if one is not specified by the user.
		/// This can be used by subclasses to provide default items to populate the list.
		/// </remarks>
		/// <returns>The default data store.</returns>
		protected virtual IEnumerable<object> CreateDefaultDataStore()
		{
			return new ListItemCollection();
		}

		/// <summary>
		/// Gets the binding to the <see cref="SelectedIndex"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<ListControl, int> SelectedIndexBinding
		{
			get
			{
				return new BindableBinding<ListControl, int>(
					this, 
					c => c.SelectedIndex, 
					(c, v) => c.SelectedIndex = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		/// <summary>
		/// Gets the binding to the <see cref="SelectedKey"/> property.
		/// </summary>
		/// <value>The selected key binding.</value>
		public BindableBinding<ListControl, string> SelectedKeyBinding
		{
			get
			{
				return new BindableBinding<ListControl, string>(
					this, 
					c => c.SelectedKey, 
					(c, v) => c.SelectedKey = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		/// <summary>
		/// Gets the binding to the <see cref="SelectedValue"/> property.
		/// </summary>
		/// <value>The selected value binding.</value>
		public BindableBinding<ListControl, object> SelectedValueBinding
		{
			get
			{
				return new BindableBinding<ListControl, object>(
					this, 
					c => c.SelectedValue, 
					(c, v) => c.SelectedValue = v, 
					(c, h) => c.SelectedValueChanged += h, 
					(c, h) => c.SelectedValueChanged -= h
				);
			}
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
		/// Callback interface for the <see cref="ListControl"/>
		/// </summary>
		public new interface ICallback : CommonControl.ICallback
		{
			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			void OnSelectedIndexChanged(ListControl widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="ListControl"/>
		/// </summary>
		protected new class Callback : CommonControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			public void OnSelectedIndexChanged(ListControl widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedIndexChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="ListControl"/>
		/// </summary>
		public new interface IHandler : CommonControl.IHandler
		{
			/// <summary>
			/// Gets or sets the data store for the items of the list control.
			/// </summary>
			/// <value>The data store.</value>
			IEnumerable<object> DataStore { get; set; }

			/// <summary>
			/// Gets or sets the index of the currently selected item in the <see cref="DataStore"/>
			/// </summary>
			/// <value>The index of the selected item.</value>
			int SelectedIndex { get; set; }

			/// <summary>
			/// Gets or sets the color of the text.
			/// </summary>
			/// <remarks>
			/// By default, the text will get a color based on the user's theme. However, this is usually black.
			/// </remarks>
			/// <value>The color of the text.</value>
			Color TextColor { get; set; }

			/// <summary>
			/// Gets or sets the binding for the text value of each item.
			/// </summary>
			/// <value>The text binding.</value>
			IIndirectBinding<string> ItemTextBinding { get; set; }

			/// <summary>
			/// Gets or sets the binding for the key value of each item.
			/// </summary>
			/// <value>The key binding.</value>
			IIndirectBinding<string> ItemKeyBinding { get; set; }
		}
	}
}
