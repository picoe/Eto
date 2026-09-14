using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class DropDownWidthTests : TestBase
	{
		static void LayOut(Control content)
		{
			// forces the template to be applied to the WPF controls, which is what measures the items
			var native = content.ToNative(false);
			native.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
			native.Arrange(new sw.Rect(native.DesiredSize));
		}

		/// <summary>
		/// Finding the widest item has to realize and measure every one of them, which for a font list is
		/// both slow and at the mercy of anything that can go wrong measuring an item. Specifying a width
		/// means none of that needs to happen until the drop down is actually opened.
		/// </summary>
		[Test, InvokeOnUI]
		public void DropDownWithWidthShouldNotMeasureItems()
		{
			var dropDown = new DropDown { Width = 100 };
			dropDown.DataStore = Enumerable.Range(0, 50).Select(r => $"Item {r}").ToList();

			var formatted = new HashSet<object>();
			dropDown.FormatItem += (sender, e) => formatted.Add(e.Item);

			LayOut(TableLayout.AutoSized(dropDown));

			var comboBox = (EtoComboBox)dropDown.ControlObject;
			Assert.That(comboBox.ItemContainerGenerator.Status, Is.Not.EqualTo(swc.Primitives.GeneratorStatus.ContainersGenerated), "#1 items should not have been realized");
			Assert.That(formatted, Is.Empty, "#2 no item should have been formatted/measured");
		}

		/// <summary>
		/// The converse of <see cref="DropDownWithWidthShouldNotMeasureItems"/> - without a width the drop
		/// down sizes itself to its widest item, which is what makes it measure all of them.
		/// </summary>
		[Test, InvokeOnUI]
		public void DropDownWithoutWidthShouldMeasureItems()
		{
			var dropDown = new DropDown();
			var items = Enumerable.Range(0, 50).Select(r => $"Item {r}").ToList();
			dropDown.DataStore = items;

			var formatted = new HashSet<object>();
			dropDown.FormatItem += (sender, e) => formatted.Add(e.Item);

			LayOut(TableLayout.AutoSized(dropDown));

			var comboBox = (EtoComboBox)dropDown.ControlObject;
			Assert.That(comboBox.ItemContainerGenerator.Status, Is.EqualTo(swc.Primitives.GeneratorStatus.ContainersGenerated), "#1 items should have been realized");
			Assert.That(formatted, Is.EquivalentTo(items), "#2 every item should have been formatted/measured");
		}
	}
}
