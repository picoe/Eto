using System;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class StackLayoutTests
	{
		[Test]
		public void AddingItemShouldSetChildrenAndParent()
		{
			TestUtils.Invoke(() =>
			{
				var stackLayout = new StackLayout();

				var items = new Control[] { new Label { ID = "label" }, new Button(), new TextBox() };

				foreach (var item in items)
					stackLayout.Items.Add(item);

				CollectionAssert.AreEqual(items, stackLayout.Children, "#1. Items do not match");

				foreach (var item in items)
					Assert.AreEqual(stackLayout, item.Parent, "#2. Items should have parent set to stack layout");

				Assert.AreSame(stackLayout.FindChild<Label>("label"), items[0], "#3. FindChild should work without loading the stack layout");

				stackLayout.Items.Clear();
				foreach (var item in items)
					Assert.IsNull(item.Parent, "#4. Items should have parent cleared when removed from stack layout");

			});
		}

		[Test]
		public void RemoveItemsIndividuallyShouldClearParent()
		{
			TestUtils.Invoke(() =>
			{
				var stackLayout = new StackLayout();

				var items = new Control[] { new Label(), new Button(), new TextBox() };

				foreach (var item in items)
					stackLayout.Items.Add(item);

				CollectionAssert.AreEqual(items, stackLayout.Children, "#1. Items do not match");

				foreach (var item in items)
					Assert.AreEqual(stackLayout, item.Parent, "#2. Items should have parent set to stack layout");

				stackLayout.Items.RemoveAt(0);
				Assert.IsNull(items[0].Parent, "#3. Item should have parent cleared when removed from stack layout");

				stackLayout.Items[0] = new Button();
				Assert.IsNull(items[1].Parent, "#4. Item should have parent cleared when replaced with another item in the stack layout");

				Assert.AreEqual(stackLayout, items[2].Parent, "#5. Item should not have changed parent as it is still in the stack layout");
			});
		}
	}
}

