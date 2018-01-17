using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class DynamicLayoutTests
	{
		[Test]
		public void AddingItemShouldSetChildrenAndParent()
		{
			TestBase.Invoke(() =>
			{
				var layout = new DynamicLayout();

				var items = new Control[] { new Label { ID = "label" }, new Button(), new TextBox() };

				foreach (var item in items)
					layout.Add(item);

				CollectionAssert.AreEqual(items, layout.Children, "#1. Items do not match");

				foreach (var item in items)
					Assert.AreEqual(layout, item.Parent, "#2. Items should have parent set to dynamic layout");

				Assert.AreSame(layout.FindChild<Label>("label"), items[0], "#3. FindChild should work without loading the dynamic layout");

				layout.Clear();
				foreach (var item in items)
					Assert.IsNull(item.Parent, "#4. Items should have parent removed when removed from dynamic layout");
			});
		}
		[Test]
		public void AddingItemMultipleLevelsDeepShouldSetChildrenAndParent()
		{
			TestBase.Invoke(() =>
			{
				var layout = new DynamicLayout();

				var items = new List<Control>();

				layout.BeginHorizontal();

				Control ctl = new Button();
				items.Add(ctl);
				layout.Add(ctl);

				layout.BeginVertical();

				ctl = new Label();
				items.Add(ctl);
				layout.Add(ctl);

				layout.EndVertical();
				layout.EndHorizontal();

				ctl = new TextBox();
				items.Add(ctl);
				layout.Add(ctl);

				CollectionAssert.AreEqual(items, layout.Children, "#1. Items do not match");

				foreach (var item in items)
					Assert.AreEqual(layout, item.Parent, "#2. Items should have parent set to dynamic layout");

				layout.Clear();
				foreach (var item in items)
					Assert.IsNull(item.Parent, "#3. Items should have parent removed when removed from dynamic layout");
			});
		}

		[Test]
		public void RemoveItemsIndividuallyShouldClearParent()
		{
			TestBase.Invoke(() =>
			{
				var layout = new DynamicLayout();

				var items = new Control[] { new Label(), new Button(), new TextBox() };

				foreach (var item in items)
					layout.Add(item);

				CollectionAssert.AreEqual(items, layout.Children, "#1. Items do not match");

				foreach (var item in items)
					Assert.AreEqual(layout, item.Parent, "#2. Items should have parent set to dynamic layout");

				layout.Rows.RemoveAt(0);
				Assert.IsNull(items[0].Parent, "#3. Item should have parent cleared when removed from dynamic layout");

				layout.Rows[0] = new Button();
				Assert.IsNull(items[1].Parent, "#4. Item should have parent cleared when replaced with another item in the dynamic layout");

				Assert.AreEqual(layout, items[2].Parent, "#5. Item should not have changed parent as it is still in the dynamic layout");
			});
		}
	}
}

