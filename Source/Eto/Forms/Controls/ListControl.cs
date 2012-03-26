using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eto.Collections;
using Eto.Drawing;
using System.Windows.Markup;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public interface IListControl : IControl
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
	}
	
	[ContentProperty("Items")]
	public class ListControl : Control
	{
		public event EventHandler<EventArgs> SelectedIndexChanged;

		IListControl inner;

		public virtual void OnSelectedIndexChanged (EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged (this, e);
		}

		protected ListControl (Generator g, Type type) : base(g, type)
		{
			inner = (IListControl)base.Handler;
		}
		
		public ListItemCollection Items {
			get {
				var items = (ListItemCollection)DataStore;
				if (items == null) {
					items = new ListItemCollection ();
					this.DataStore = items;
				}
				return items;
			}
			
		}
		
		public IListStore DataStore {
			get { return inner.DataStore; }
			set { inner.DataStore = value; }
		}

		public int SelectedIndex {
			get { return inner.SelectedIndex; }
			set { inner.SelectedIndex = value; }
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
		
	}
}
