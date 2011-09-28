using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eto.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IListControl : IControl
	{
		void AddRange(IEnumerable<IListItem> collection);
		void AddItem(IListItem item);
		void RemoveItem(IListItem item);
		void RemoveAll();
		int SelectedIndex { get; set; }
	}
	
	public class ListControl : Control
	{
		public event EventHandler<EventArgs> SelectedIndexChanged;
		ItemCollection items;
		
		private IListControl inner;

		public class ItemCollection : BaseList<IListItem>
		{
			internal ListControl Handler { get; set; }
			
			public override void AddRange (IEnumerable<IListItem> collection)
			{
				base.AddRange (collection);
				Handler.inner.AddRange(collection);
			}

			public void AddRange (IEnumerable<object> collection)
			{
				var list = (from r in collection select 
					(r is IListItem ? (IListItem)r : new ObjectListItem{ Item = r })
				).ToArray ();
				base.AddRange (list);
				Handler.inner.AddRange(list);
			}

			public void Add (object value)
			{
				var item = value as IListItem;
				if (item != null) base.Add (item);
				else base.Add (new ObjectListItem { Item = value });
			}
			
			protected override void OnAdded (ListEventArgs<IListItem> e)
			{
				base.OnAdded (e);
				Handler.inner.AddItem(e.Item);
			}

			protected override void OnRemoved (ListEventArgs<IListItem> e)
			{
				base.OnRemoved (e);
				Handler.inner.RemoveItem(e.Item);
			}
			
			public override void Clear ()
			{
				base.Clear ();
				Handler.inner.RemoveAll();
			}
		}

		public virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, e);
		}

		protected ListControl(Generator g, Type type) : base(g, type)
		{
			inner = (IListControl)base.Handler;
			items = new ItemCollection{ Handler = this };
		}

		public ItemCollection Items
		{
			get { return items; }
		}

		public int SelectedIndex
		{
			get { return inner.SelectedIndex; }
			set { inner.SelectedIndex = value; }
		}

		public IListItem SelectedValue
		{
			get { return (SelectedIndex >= 0) ? Items[SelectedIndex] : null; }
			set { SelectedIndex = Items.IndexOf(value); }
		}
		
		public string SelectedKey
		{
			get { return (SelectedIndex >= 0) ? Items[SelectedIndex].Key : null; }
			set { 
				var val = Items.FirstOrDefault(r => r.Key == value);
				if (val != null) SelectedIndex = Items.IndexOf(val);
			}
		}
		
	}
}
