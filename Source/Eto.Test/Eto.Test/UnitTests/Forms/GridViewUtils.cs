using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using System.Collections.ObjectModel;

namespace Eto.Test.UnitTests.Forms
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
			return "Item " + Id;
		}
	}

	static class GridViewUtils
	{
		public const int ItemCount = 100;

		/// <summary>
		/// Creates a model with 100 items, with index as Id.
		/// </summary>
		public static ObservableCollection<DataItem> CreateModel()
		{
			var model = new ObservableCollection<DataItem>();
			for (var i = 0; i < ItemCount; ++i)
				model.Add(new DataItem(i));
			return model;
		}

		public static int SortEvenItemsBeforeOdd(DataItem x, DataItem y)
		{
			var a = x.Id;
			var b = y.Id;
			return (a % 2 == b % 2) ? a - b : (a % 2 == 0 ? -1 : 1);
		}

		public static int SortItemsAscending(DataItem x, DataItem y)
		{
			var a = x.Id;
			var b = y.Id;
			return a - b;
		}

		public static int SortItemsDescending(DataItem x, DataItem y)
		{
			return -SortItemsAscending(x, y);
		}

		public static bool KeepOddItemsFilter(DataItem o)
		{
			return o.Id % 2 == 1; // keep all odd items only.
		}

		public static bool KeepFirstHalfOfItemsFilter(DataItem o)
		{
			return o.Id < ItemCount / 2;
		}
	}
}