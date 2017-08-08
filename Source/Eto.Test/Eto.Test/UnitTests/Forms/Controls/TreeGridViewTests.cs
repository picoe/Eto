using System;
using NUnit.Framework;
using Eto.Forms;
using System.Linq;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TreeGridViewTests : TestBase
	{
		[Test]
		public void SelectedItemsShouldNotCrash()
		{
			Invoke(() =>
			{
				var tree = new TreeGridView();

				tree.DataStore = new TreeGridItemCollection();

				var items = tree.SelectedItems.ToList();
				Assert.IsNotNull(items);
				Assert.AreEqual(0, items.Count);
			});
		}
	}
}
