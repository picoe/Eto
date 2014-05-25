using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IListStore : IDataStore<IListItem>
	{
	}

	public class ListItemCollection : DataStoreCollection<IListItem>, IListStore
	{
		public void Add(string text)
		{
			base.Add(new ListItem{ Text = text });
		}

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
			return item != null ? item.Text : base.InternalGetValue(dataItem);
		}

		protected override void InternalSetValue(object dataItem, string value)
		{
			var item = dataItem as IListItem;
			if (item != null)
				item.Text = Convert.ToString(value);
			else
				base.InternalSetValue(dataItem, value);
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
			return item != null ? item.Key : base.InternalGetValue(dataItem);
		}
	}

	[ContentProperty("Items")]
	public abstract class ListControl : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public IIndirectBinding<string> TextBinding { get; set; }

		public IIndirectBinding<string> KeyBinding { get; set; }

		public event EventHandler<EventArgs> SelectedIndexChanged;

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, e);
			OnSelectedValueChanged(e);
		}

		public event EventHandler<EventArgs> SelectedValueChanged;

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			if (SelectedValueChanged != null)
				SelectedValueChanged(this, e);
		}

		protected ListControl()
		{
			TextBinding = new ListItemTextBinding();
			KeyBinding = new ListItemKeyBinding();
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ListControl(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			TextBinding = new ListItemTextBinding();
			KeyBinding = new ListItemKeyBinding();
		}

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

		public IEnumerable<object> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		public int SelectedIndex
		{
			get { return Handler.SelectedIndex; }
			set { Handler.SelectedIndex = value; }
		}

		public object SelectedValue
		{
			get { return (SelectedIndex >= 0 && Handler.DataStore != null) ? Handler.DataStore.StoreElementAt(SelectedIndex) : null; }
			set
			{
				EnsureDataStore();
				SelectedIndex = Handler.DataStore != null ? Handler.DataStore.IndexOf(value) : -1;
			}
		}

		public string SelectedKey
		{
			get { return KeyBinding.GetValue(SelectedValue); }
			set
			{
				EnsureDataStore();
				SelectedIndex = Handler.DataStore != null ? Handler.DataStore.FindIndex(r => KeyBinding.GetValue(r) == value) : -1;
			}
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			EnsureDataStore();
		}

		[Obsolete("Use CreateDefaultDataStore")]
		protected virtual ListItemCollection CreateDefaultItems()
		{
			return new ListItemCollection();
		}

		internal void EnsureDataStore()
		{
			if (DataStore == null)
				DataStore = CreateDefaultDataStore();
		}


		protected virtual IEnumerable<object> CreateDefaultDataStore()
		{
			#pragma warning disable 612,618
			return CreateDefaultItems();
			#pragma warning restore 612,618
		}

		public ObjectBinding<ListControl, int> SelectedIndexBinding
		{
			get
			{
				return new ObjectBinding<ListControl, int>(
					this, 
					c => c.SelectedIndex, 
					(c, v) => c.SelectedIndex = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		public ObjectBinding<ListControl, string> SelectedKeyBinding
		{
			get
			{
				return new ObjectBinding<ListControl, string>(
					this, 
					c => c.SelectedKey, 
					(c, v) => c.SelectedKey = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		public ObjectBinding<ListControl, object> SelectedValueBinding
		{
			get
			{
				return new ObjectBinding<ListControl, object>(
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

		public new interface ICallback : CommonControl.ICallback
		{
			void OnSelectedIndexChanged(ListControl widget, EventArgs e);
		}

		protected new class Callback : CommonControl.Callback, ICallback
		{
			public void OnSelectedIndexChanged(ListControl widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnSelectedIndexChanged(e));
			}
		}

		public new interface IHandler : CommonControl.IHandler
		{
			IEnumerable<object> DataStore { get; set; }

			int SelectedIndex { get; set; }
		}
	}
}
