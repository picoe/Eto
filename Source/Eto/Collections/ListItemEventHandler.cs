using System;

namespace Eto.Collections
{
	public delegate void ListEventHandler<T> (object sender,ListEventArgs<T> e);
	
	public class ListEventArgs<T> : EventArgs
	{
		
		public T Item { get; private set; }
		
		public int Index { get; private set; }
		
		public ListEventArgs (T item, int index)
		{
			this.Item = item;
			this.Index = index;
		}
	}
}
