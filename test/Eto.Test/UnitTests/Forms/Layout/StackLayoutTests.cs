using System;
using Eto.Forms;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class StackLayoutTests : TestBase
	{
		[Test]
		public void AddingItemShouldSetChildrenAndParent()
		{
			Invoke(() =>
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
			Invoke(() =>
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

		[Test]
		public void LogicalParentOfChildrenShouldBeStackLayout()
		{
			StackLayout stack = null;
			Panel child = null;
			Shown(form =>
			{
				child = new Panel();
				stack = new StackLayout
				{
					Items = { child }
				};
				Assert.AreSame(stack, child.Parent);
				Assert.IsNull(child.VisualParent);
				form.Content = stack;
			}, () =>
			{
				Assert.AreSame(stack, child?.Parent);
				Assert.IsNotNull(child.VisualParent);
				// StackLayout uses TableLayout internally to align controls
				// this will be changed when StackLayout does not depend on TableLayout
				Assert.AreNotSame(stack, child.VisualParent);
				Assert.IsInstanceOf<TableLayout>(child.VisualParent);
			});
		}

		[Test]
		public void LogicalParentShouldChangeWhenAddedOrRemoved()
		{
			Invoke(() =>
			{
				var child = new Panel();
				var stack = new StackLayout();
				stack.Items.Add(child);
				Assert.AreSame(stack, child.Parent);
				stack.Items.Clear();
				Assert.IsNull(child.Parent);
				stack.Items.Add(child);
				Assert.AreSame(stack, child.Parent);
				stack.Items.RemoveAt(0);
				Assert.IsNull(child.Parent);
				stack.Items.Insert(0, child);
				Assert.AreSame(stack, child.Parent);
				stack.Items[0] = new StackLayoutItem();
				Assert.IsNull(child.Parent);
			});
		}

		[Test]
		public void LogicalParentShouldChangeWhenAddedOrRemovedWhenLoaded()
		{
			Shown(form => new StackLayout(), stack =>
			{
				var child = new Panel();
				stack.Items.Add(child);
				Assert.IsNotNull(child.VisualParent);
				Assert.IsInstanceOf<TableLayout>(child.VisualParent);
				Assert.AreSame(stack, child.Parent);
				stack.Items.Clear();
				Assert.IsNull(child.VisualParent);
				Assert.IsNull(child.Parent);
				stack.Items.Add(child);
				Assert.IsNotNull(child.VisualParent);
				Assert.IsInstanceOf<TableLayout>(child.VisualParent);
				Assert.AreSame(stack, child.Parent);
				stack.Items.RemoveAt(0);
				Assert.IsNull(child.VisualParent);
				Assert.IsNull(child.Parent);
				stack.Items.Insert(0, child);
				Assert.IsNotNull(child.VisualParent);
				Assert.IsInstanceOf<TableLayout>(child.VisualParent);
				Assert.AreSame(stack, child.Parent);
				stack.Items[0] = new StackLayoutItem();
				Assert.IsNull(child.VisualParent);
				Assert.IsNull(child.Parent);
			});
		}

		[Test, ManualTest]
		public void UpdateShouldKeepAlignment()
		{
			ManualForm(
				"Label should stay centered vertically after clicking the button",
				form =>
				{
					StackLayout content = null;
					Action command = () =>
					{
						if (content == null)
							return;
						content.Items[1] = new ComboBox { Items = { "Zus", "Wim", "Jet" }, SelectedIndex = 1 };
					};

					return content = new StackLayout
					{
						VerticalContentAlignment = VerticalAlignment.Center,
						Orientation = Orientation.Horizontal,
						Height = 100, // so we can exaggerate the issue
						Items =
						{
							"Hello",
							new ComboBox { Items = { "Aap", "Noot", "Mies" }, SelectedIndex = 1 },
							"There",
							new Button
							{
								Text = "Click",
								Command = new RelayCommand(command)
							}
						}
					};
				});
		}
	}
}