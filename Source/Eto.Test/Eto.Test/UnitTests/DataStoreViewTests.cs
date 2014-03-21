using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using Assert = NUnit.Framework.Assert;
using Eto.Forms;
using Eto;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Unit tests for DataStoreView
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	[TestClass]
	public class DataStoreViewTests
	{
		static int ItemCount { get { return GridViewUtils.ItemCount; } }
		DataStoreCollection model;
		DataStoreView d;

		[TestInitialize]
		public void Setup()
		{
			model = GridViewUtils.CreateModel();
			d = new DataStoreView { Model = model };
		}

		[TestMethod]
		public void DataStoreView_WithNoFilterOrSort_ViewContainsAllModelItems()
		{
			var view = d.View;
			Assert.AreEqual(ItemCount, view.Count);
			for (var i = 0; i < ItemCount; ++i)
				Assert.AreSame(model[i], view[i]);
		}

		[TestMethod]
		public void DataStoreView_WithOddItemFilter_ViewContainsOddModelItems()
		{
			d.Filter = GridViewUtils.KeepOddItemsFilter;
			var view = d.View;
			Assert.AreEqual(ItemCount / 2, view.Count);
			for (var i = 0; i < ItemCount / 2; ++i)
				Assert.AreSame(model[i * 2 + 1], view[i]);
		}
	
		[TestMethod]
		public void DataStoreView_SortWithEvenItemsBeforeOddItems_SortsCorrectly()
		{
			d.SortComparer = GridViewUtils.SortEvenItemsBeforeOdd;
			var view = d.View;
			Assert.AreEqual(ItemCount, view.Count);
			for (var i = 0; i < ItemCount/2; ++i)
				Assert.AreSame(model[i * 2], view[i]);
			for (var i = 0; i < ItemCount / 2; ++i)
				Assert.AreSame(model[i * 2 + 1], view[ItemCount/2 + i]);
		}

		[TestMethod]
		public void DataStoreView_SortAscending_SortsCorrectly()
		{
			d.SortComparer = GridViewUtils.SortItemsAscending;
			var view = d.View;
			Assert.AreEqual(ItemCount, view.Count);
			for (var i = 0; i < ItemCount; ++i)
				Assert.AreSame(model[i], view[i]);
		}

		[TestMethod]
		public void DataStoreView_SortDescending_SortsCorrectly()
		{
			d.SortComparer = GridViewUtils.SortItemsDescending;
			var view = d.View;
			Assert.AreEqual(ItemCount, view.Count);
			for (var i = 0; i < ItemCount; ++i)
				Assert.AreSame(model[ItemCount - 1 - i], view[i]);
		}

		[TestMethod]
		public void DataStoreView_SortWithEvenItemsBeforeOddItemsAndWithFilter_SortsAndFiltersCorrectly()
		{
			d.SortComparer = GridViewUtils.SortEvenItemsBeforeOdd;
			d.Filter = GridViewUtils.KeepFirstHalfOfItemsFilter;
			var view = d.View;
			Assert.AreEqual(ItemCount/2, view.Count);
			for (var i = 0; i < ItemCount / 4; ++i)
				Assert.AreSame(model[i * 2], view[i]);
			for (var i = 0; i < ItemCount / 4; ++i)
				Assert.AreSame(model[i * 2 + 1], view[ItemCount / 4 + i]);
		}
	}
}
