using NUnit.Framework;
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

				Assert.That(layout.Children, Is.EqualTo(items), "#1. Items do not match");

				foreach (var item in items)
					Assert.That(item.Parent, Is.EqualTo(layout), "#2. Items should have parent set to dynamic layout");

				Assert.That(layout.FindChild<Label>("label"), Is.SameAs(items[0]), "#3. FindChild should work without loading the dynamic layout");

				layout.Clear();
				foreach (var item in items)
					Assert.That(item.Parent, Is.Null, "#4. Items should have parent removed when removed from dynamic layout");
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

				Assert.That(layout.Children, Is.EqualTo(items), "#1. Items do not match");

				foreach (var item in items)
					Assert.That(item.Parent, Is.EqualTo(layout), "#2. Items should have parent set to dynamic layout");

				layout.Clear();
				foreach (var item in items)
					Assert.That(item.Parent, Is.Null, "#3. Items should have parent removed when removed from dynamic layout");
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

				Assert.That(layout.Children, Is.EqualTo(items), "#1. Items do not match");

				foreach (var item in items)
					Assert.That(item.Parent, Is.EqualTo(layout), "#2. Items should have parent set to dynamic layout");

				layout.Rows.RemoveAt(0);
				Assert.That(items[0].Parent, Is.Null, "#3. Item should have parent cleared when removed from dynamic layout");

				layout.Rows[0] = new Button();
				Assert.That(items[1].Parent, Is.Null, "#4. Item should have parent cleared when replaced with another item in the dynamic layout");

				Assert.That(items[2].Parent, Is.EqualTo(layout), "#5. Item should not have changed parent as it is still in the dynamic layout");
			});
		}
	}
}

