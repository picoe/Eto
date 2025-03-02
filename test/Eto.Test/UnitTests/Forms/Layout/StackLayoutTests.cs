using NUnit.Framework;
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
	}
}