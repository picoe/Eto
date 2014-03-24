#if !NO_UNITTESTS
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using NUnit.Framework;
using Eto.Forms;
using Eto;
using Eto.Test.UnitTests.Handlers;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Unit tests for GridView
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	[TestFixture]
	public class GridViewTests
	{
		[Test]
		public void GridView_SetFilterBeforeDataStore_NoException()
		{
			var g = new GridView(null, new TestGridViewHandler());
			g.Filter = GridViewUtils.KeepOddItemsFilter; 
			g.DataStore = GridViewUtils.CreateModel();
		}

		[Test]
		public void GridView_SetSortBeforeDataStore_NoException()
		{
			var g = new GridView(null, new TestGridViewHandler());
			g.SortComparer = GridViewUtils.SortItemsAscending;
			g.DataStore = GridViewUtils.CreateModel();
		}
	}
}
#endif