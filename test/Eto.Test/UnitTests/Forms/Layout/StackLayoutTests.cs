using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class StackLayoutTests : TestBase
	{
		[Test, ManualTest]
		public void WindowShouldShrinkToFitContents()
		{
			ManualForm(
				"WindowShouldShrinkToFitContents",
				form =>
				{
					var stackLayout = new StackLayout();
					form.AutoSize = true;
					Application.Instance.InvokeAsync(() =>
					{
						stackLayout.Items.Add(
							new Label
							{
								Text = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit...",
							}
						);
						stackLayout.Items.Add(
							new Panel
							{
								BackgroundColor = Colors.Thistle,
								Width = 2,
								Height = 2,
							}
						);
					});
					return stackLayout;
				});
		}

		[Test]
		public void AddingItemShouldSetChildrenAndParent()
		{
			Invoke(() =>
			{
				var stackLayout = new StackLayout();

				var items = new Control[] { new Label { ID = "label" }, new Button(), new TextBox() };

				foreach (var item in items)
					stackLayout.Items.Add(item);

				Assert.That(stackLayout.Children, Is.EqualTo(items), "#1. Items do not match");

				foreach (var item in items)
					Assert.That(item.Parent, Is.EqualTo(stackLayout), "#2. Items should have parent set to stack layout");

				Assert.That(stackLayout.FindChild<Label>("label"), Is.SameAs(items[0]), "#3. FindChild should work without loading the stack layout");

				stackLayout.Items.Clear();
				foreach (var item in items)
					Assert.That(item.Parent, Is.Null, "#4. Items should have parent cleared when removed from stack layout");

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

				Assert.That(stackLayout.Children, Is.EqualTo(items), "#1. Items do not match");

				foreach (var item in items)
					Assert.That(item.Parent, Is.EqualTo(stackLayout), "#2. Items should have parent set to stack layout");

				stackLayout.Items.RemoveAt(0);
				Assert.That(items[0].Parent, Is.Null, "#3. Item should have parent cleared when removed from stack layout");

				stackLayout.Items[0] = new Button();
				Assert.That(items[1].Parent, Is.Null, "#4. Item should have parent cleared when replaced with another item in the stack layout");

				Assert.That(items[2].Parent, Is.EqualTo(stackLayout), "#5. Item should not have changed parent as it is still in the stack layout");
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
				Assert.That(stack, Is.SameAs(child.Parent));
				Assert.That(child.VisualParent, Is.Null);
				form.Content = stack;
			}, () =>
			{
				Assert.That(stack, Is.SameAs(child?.Parent));
				Assert.That(child.VisualParent, Is.Not.Null);
				// StackLayout uses TableLayout internally to align controls
				// this will be changed when StackLayout does not depend on TableLayout
				Assert.That(stack, Is.Not.SameAs(child.VisualParent));
				Assert.That(child.VisualParent, Is.InstanceOf<TableLayout>());
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
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items.Clear();
				Assert.That(child.Parent, Is.Null);
				stack.Items.Add(child);
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items.RemoveAt(0);
				Assert.That(child.Parent, Is.Null);
				stack.Items.Insert(0, child);
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items[0] = new StackLayoutItem();
				Assert.That(child.Parent, Is.Null);
			});
		}

		[Test]
		public void LogicalParentShouldChangeWhenAddedOrRemovedWhenLoaded()
		{
			Shown(form => new StackLayout(), stack =>
			{
				var child = new Panel();
				stack.Items.Add(child);
				Assert.That(child.VisualParent, Is.Not.Null);
				Assert.That(child.VisualParent, Is.InstanceOf<TableLayout>());
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items.Clear();
				Assert.That(child.VisualParent, Is.Null);
				Assert.That(child.Parent, Is.Null);
				stack.Items.Add(child);
				Assert.That(child.VisualParent, Is.Not.Null);
				Assert.That(child.VisualParent, Is.InstanceOf<TableLayout>());
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items.RemoveAt(0);
				Assert.That(child.VisualParent, Is.Null);
				Assert.That(child.Parent, Is.Null);
				stack.Items.Insert(0, child);
				Assert.That(child.VisualParent, Is.Not.Null);
				Assert.That(child.VisualParent, Is.InstanceOf<TableLayout>());
				Assert.That(stack, Is.SameAs(child.Parent));
				stack.Items[0] = new StackLayoutItem();
				Assert.That(child.VisualParent, Is.Null);
				Assert.That(child.Parent, Is.Null);
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

		[Test]
		public void DetachingChildShouldRemoveItem()
		{
			Invoke(() =>
			{
				var child1 = new Label();
				var child2 = new Button();
				var stack = new StackLayout { Items = { child1, child2 } };

				child1.Detach();

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#1 - Item should be removed from Items");
				Assert.That(stack.Items[0].Control, Is.SameAs(child2), "#2 - Remaining item should be the other child");
				Assert.That(stack.Controls, Is.EqualTo(new Control[] { child2 }), "#3 - Controls should not include the detached child");
				Assert.That(child1.Parent, Is.Null, "#4 - Detached child should not have a parent");
				Assert.That(child2.Parent, Is.SameAs(stack), "#5 - Other child should still have the stack as its parent");
			});
		}

		[Test]
		public void DetachingChildShouldRemoveItemWhenLoaded()
		{
			Label child1 = null;
			Button child2 = null;
			int unloadCount = 0;
			Shown(form =>
			{
				child1 = new Label { Text = "Child 1" };
				child2 = new Button { Text = "Child 2" };
				child1.UnLoad += (sender, e) => unloadCount++;
				return new StackLayout { Items = { child1, child2 } };
			}, stack =>
			{
				Assert.That(child1.VisualParent, Is.Not.Null, "#1 - Child should be in the visual tree");

				child1.Detach();

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#2 - Item should be removed from Items");
				Assert.That(child1.Parent, Is.Null, "#3 - Detached child should not have a parent");
				Assert.That(child1.VisualParent, Is.Null, "#4 - Detached child should not be in the visual tree");
				Assert.That(child1.Loaded, Is.False, "#5 - Detached child should be unloaded");
				Assert.That(unloadCount, Is.EqualTo(1), "#6 - UnLoad should be raised once");
				Assert.That(child2.VisualParent, Is.Not.Null, "#7 - Other child should still be in the visual tree");
			});
		}

		[Test]
		public void RemoveShouldRemoveItem()
		{
			Invoke(() =>
			{
				var child1 = new Label();
				var child2 = new Button();
				var stack = new StackLayout { Items = { child1, child2 } };

				stack.Remove(child2);

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#1 - Item should be removed from Items");
				Assert.That(stack.Items[0].Control, Is.SameAs(child1), "#2 - Remaining item should be the other child");
				Assert.That(child2.Parent, Is.Null, "#3 - Removed child should not have a parent");

				// removing a control that isn't in the stack should do nothing
				stack.Remove(new Label());
				stack.Remove((Control)null);
				Assert.That(stack.Items.Count, Is.EqualTo(1), "#4 - Removing a control not in the stack should not change Items");
			});
		}

		[Test]
		public void DisposingChildShouldRemoveItem()
		{
			Shown(form => new StackLayout { Items = { new Label { Text = "Child 1" }, new Button { Text = "Child 2" } } }, stack =>
			{
				var child1 = stack.Items[0].Control;

				child1.Dispose();

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#1 - Disposed child should be removed from Items");
				Assert.That(stack.Controls.Any(c => c.IsDisposed), Is.False, "#2 - Controls should not include disposed children");

				// disposing the stack should not touch the disposed child
				Assert.DoesNotThrow(() => stack.Dispose(), "#3 - Disposing the stack should not throw");
			});
		}

		[Test]
		public void MovingChildToAnotherContainerShouldRemoveItem()
		{
			Invoke(() =>
			{
				var child = new Label();
				var stack = new StackLayout { Items = { child, new Button() } };
				var panel = new Panel();

				panel.Content = child;

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#1 - Child should be removed from the stack when moved to another container");
				Assert.That(stack.Controls.Contains(child), Is.False, "#2 - Stack should not contain the moved child");
				Assert.That(child.Parent, Is.SameAs(panel), "#3 - Child should have the new container as its parent");
			});
		}

		[Test]
		public void SettingSameControlShouldKeepParent()
		{
			Invoke(() =>
			{
				var child = new Label();
				var stack = new StackLayout { Items = { child } };

				stack.Items[0] = new StackLayoutItem(child, HorizontalAlignment.Right);

				Assert.That(stack.Items.Count, Is.EqualTo(1), "#1 - Item should be replaced, not removed");
				Assert.That(stack.Items[0].Control, Is.SameAs(child), "#2 - Item should still have the same control");
				Assert.That(child.Parent, Is.SameAs(stack), "#3 - Child should still have the stack as its parent");
			});
		}
	}
}