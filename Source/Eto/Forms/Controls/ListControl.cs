using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.Collections.ObjectModel;
#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface IListControl : ICommonControl
	{
		IListStore DataStore { get; set; }
		
		int SelectedIndex { get; set; }
	}
	
	public interface IListStore : IDataStore<IListItem>
	{
	}
	
	public class ListItemCollection : DataStoreCollection<IListItem>, IListStore
	{
		public void Add (string text)
		{
			base.Add (new ListItem{ Text = text });
		}

		public void Add (string text, string key)
		{
			base.Add (new ListItem { Text = text, Key = key });
		}
	}
	
#if XAML
	[ContentProperty("Items")]
#endif
	public class ListControl : CommonControl
	{
		public event EventHandler<EventArgs> SelectedIndexChanged;

		IListControl handler;

		public virtual void OnSelectedIndexChanged (EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged (this, e);
		}

		protected ListControl (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (IListControl)base.Handler;
		}
		
		public ListItemCollection Items {
			get {
				var items = (ListItemCollection)DataStore;
				if (items == null) {
					items = CreateDefaultItems();
					this.DataStore = items;
				}
				return items;
			}
		}

		public IListStore DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}

		public int SelectedIndex {
			get { return handler.SelectedIndex; }
			set { handler.SelectedIndex = value; }
		}

		public IListItem SelectedValue {
			get { return (SelectedIndex >= 0) ? Items [SelectedIndex] : null; }
			set { SelectedIndex = Items.IndexOf (value); }
		}
		
		public string SelectedKey {
			get { return (SelectedIndex >= 0) ? Items [SelectedIndex].Key : null; }
			set { 
				var val = Items.FirstOrDefault (r => r.Key == value);
				if (val != null)
					SelectedIndex = Items.IndexOf (val);
			}
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			if (DataStore == null)
				this.DataStore = CreateDefaultItems ();
		}

		protected virtual ListItemCollection CreateDefaultItems ()
		{
			return new ListItemCollection ();
		}
	}
}
