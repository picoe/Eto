using System;
using System.Collections.Generic;
using System.Collections;
using Eto.Forms;
using System.Linq;

namespace Eto
{
	static class EnumerableExtensions
	{
		public static T StoreElementAt<T>(this IEnumerable<T> items, int index)
		{
			var iDataStore = items as IDataStore<T>;
			if (iDataStore != null)
			{
				return iDataStore[index];
			}
			return items.ElementAt(index);
		}

		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> match)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			if (match == null)
				throw new ArgumentNullException("match");

			int retVal = 0;
			foreach (var item in items)
			{
				if (match(item))
					return retVal;
				retVal++;
			}
			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> items, T item)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			var list = items as IList<T>;
			if (list != null)
				return list.IndexOf(item);
			var list2 = items as IList;
			if (list2 != null)
				return list2.IndexOf(item);
			return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
		}
	}
}