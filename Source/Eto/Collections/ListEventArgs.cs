using System;

namespace Eto.Collections
{
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
