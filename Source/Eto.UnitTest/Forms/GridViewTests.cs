using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using NUnit.Framework;
using Eto.Forms;
using Eto;
using Eto.UnitTest.Handlers;

namespace Eto.UnitTest.Forms
{
	/// <summary>
	/// Unit tests for GridView
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class GridViewTests
	{
		[Test, Invoke]
		public void GridView_SetFilterBeforeDataStore_NoException()
		{
			var g = new GridView();
			g.Filter = GridViewUtils.KeepOddItemsFilter; 
			g.DataStore = GridViewUtils.CreateModel();
		}

		[Test, Invoke]
		public void GridView_SetSortBeforeDataStore_NoException()
		{
			var g = new GridView();
			g.SortComparer = GridViewUtils.SortItemsAscending;
			g.DataStore = GridViewUtils.CreateModel();
		}
	}
}