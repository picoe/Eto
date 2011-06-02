using System;

namespace Eto.Collections
{
	public delegate void ListEventHandler<T>(object sender, ListEventArgs<T> e);
	
	public class ListEventArgs<T> : EventArgs
	{
		T item;
		
		public T Item
		{
			get { return item; }
		}
		
		public ListEventArgs(T item)
		{
			this.item = item;
		}
	}
}
