using System;
using System.Linq;

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

	[ContentProperty("Items")]
	public abstract class ListControl : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ListControl(Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
		
		public ListItemCollection Items
		{
			get
			{
				var items = (ListItemCollection)DataStore;
				if (items == null)
				{
					items = CreateDefaultItems();
					DataStore = items;
				}
				return items;
			}
		}

		public IListStore DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		public int SelectedIndex
		{
			get { return Handler.SelectedIndex; }
			set { Handler.SelectedIndex = value; }
		}

		public IListItem SelectedValue
		{
			get { return (SelectedIndex >= 0) ? Items[SelectedIndex] : null; }
			set { SelectedIndex = Items.IndexOf(value); }
		}
		
		public string SelectedKey
		{
			get { return (SelectedIndex >= 0) ? Items[SelectedIndex].Key : null; }
			set
			{ 
				var val = Items.FirstOrDefault(r => r.Key == value);
				if (val != null)
					SelectedIndex = Items.IndexOf(val);
			}
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (DataStore == null)
				DataStore = CreateDefaultItems();
		}

		protected virtual ListItemCollection CreateDefaultItems()
		{
			return new ListItemCollection();
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

		public ObjectBinding<ListControl, IListItem> SelectedValueBinding
		{
			get
			{
				return new ObjectBinding<ListControl, IListItem>(
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
		protected override object GetCallback() { return callback; }

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
			IListStore DataStore { get; set; }

			int SelectedIndex { get; set; }
		}
	}
}
