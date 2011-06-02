using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Collections;

namespace Eto.Forms
{
	public interface IListControl : IControl
	{
		void AddRange(IEnumerable<object> collection);
		void AddItem(object item);
		void RemoveItem(object item);
		void RemoveAll();
		int SelectedIndex { get; set; }
	}
	
	public class ListControl : Control
	{
		public event EventHandler<EventArgs> SelectedIndexChanged;
		ItemCollection items;
		
		private IListControl inner;

		public class ItemCollection : BaseList<object>
		{
			internal ListControl Handler { get; set; }
			
			public override void AddRange (IEnumerable<object> collection)
			{
				base.AddRange (collection);
				Handler.inner.AddRange(collection);
			}
			
			protected override void OnAdded (ListEventArgs<object> e)
			{
				base.OnAdded (e);
				Handler.inner.AddItem(e.Item);
			}

			protected override void OnRemoved (ListEventArgs<object> e)
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

		public object SelectedValue
		{
			get { return (SelectedIndex >= 0) ? Items[SelectedIndex] : null; }
			set { SelectedIndex = Items.IndexOf(value); }
		}
	}
}
