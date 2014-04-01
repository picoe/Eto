using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace Eto.UnitTest.Forms
{
	class DataItem
	{
		public int Id { get; private set; }

		public DataItem(int id)
		{
			Id = id;
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}

	static class GridViewUtils
	{
		public const int ItemCount = 100;

		/// <summary>
		/// Creates a model with 100 items, with index as Id.
		/// </summary>
		public static DataStoreCollection CreateModel()
		{
			var model = new DataStoreCollection();
			for (var i = 0; i < ItemCount; ++i)
				model.Add(new DataItem(i));
			return model;
		}

		public static int SortEvenItemsBeforeOdd(object x, object y)
		{
			var a = ((DataItem)x).Id;
			var b = ((DataItem)y).Id;
			return (a % 2 == b % 2) ? a - b : (a % 2 == 0 ? -1 : 1);
		}

		public static int SortItemsAscending(object x, object y)
		{
			var a = ((DataItem)x).Id;
			var b = ((DataItem)y).Id;
			return a - b;
		}

		public static int SortItemsDescending(object x, object y)
		{
			return -SortItemsAscending(x, y);
		}

		public static bool KeepOddItemsFilter(object o)
		{
			return ((DataItem)o).Id % 2 == 1; // keep all odd items only.
		}

		public static bool KeepFirstHalfOfItemsFilter(object o)
		{
			return ((DataItem)o).Id < ItemCount / 2;
		}
	}
}