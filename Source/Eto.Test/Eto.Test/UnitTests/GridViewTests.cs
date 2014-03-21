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
	/// Unit tests for GridView
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	[TestClass]
	public class GridViewTests
	{
		[TestMethod]
		public void GridView_SetFilterBeforeDataStore_NoException()
		{
			var g = new GridView(null, new TestGridViewHandler());
			g.Filter = GridViewUtils.KeepOddItemsFilter; 
			g.DataStore = GridViewUtils.CreateModel();
		}

		[TestMethod]
		public void GridView_SetSortBeforeDataStore_NoException()
		{
			var g = new GridView(null, new TestGridViewHandler());
			g.SortComparer = GridViewUtils.SortItemsAscending;
			g.DataStore = GridViewUtils.CreateModel();
		}
	}
}
