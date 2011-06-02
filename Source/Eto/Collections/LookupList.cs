using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Collections
{
	public abstract class LookupList<INDEX, VALUE> : BaseList<VALUE>
	{
		protected override void OnAdded (ListEventArgs<VALUE> e)
		{
			base.OnAdded (e);
			lookup.Add(GetLookupByValue(e.Item), e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<VALUE> e)
		{
			base.OnRemoved (e);
			lookup.Remove(GetLookupByValue(e.Item));
		}
		
		protected abstract INDEX GetLookupByValue(VALUE value);
		
		protected virtual INDEX GetLookupByIndex(INDEX index)
		{
			return index;
		}
		
		
		public override void Clear()
		{
			base.Clear();
			lookup.Clear();
		}
		
		public bool ContainsKey(INDEX index)
		{
			return lookup.ContainsKey(GetLookupByIndex(index));
		}
		
		public override bool Contains(VALUE item)
		{
			return lookup.ContainsKey(GetLookupByValue(item));
		}
		
		Dictionary<INDEX, VALUE> lookup;
		
		public LookupList()
		{
			lookup = new Dictionary<INDEX, VALUE>();
		}

		public VALUE this[INDEX index]
		{
			get
			{
				return lookup[GetLookupByIndex(index)];
				//else return default(VALUE);
			}
		}
		
		
		public VALUE Find(INDEX index)
		{
			INDEX lookupIndex = GetLookupByIndex(index);
			if (lookup.ContainsKey(lookupIndex)) return lookup[lookupIndex];
			else return default(VALUE);
		}
	}
}


