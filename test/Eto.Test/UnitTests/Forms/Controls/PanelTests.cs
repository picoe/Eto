using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class PanelTests : TestBase
	{
		[Test]
		public void ParentShouldBeSet()
		{
			Invoke(() =>
			{
				var panel1 = new Panel { ID = "panel1" };
				var panel2 = new Panel { ID = "panel2" };
				var label = new Label { Text = "Label" };

				panel1.Content = label;
				Assert.That(panel1, Is.SameAs(label.Parent), "#1");
				Assert.That(panel1, Is.SameAs(label.VisualParent), "#2");

				panel1.Content = null;
				Assert.That(null, Is.SameAs(label.Parent), "#2");
				Assert.That(null, Is.SameAs(label.VisualParent), "#3");

				panel2.Content = label;
				Assert.That(panel2, Is.SameAs(label.Parent), "#3");
				Assert.That(panel2, Is.SameAs(label.VisualParent), "#4");
			});
		}
	}
}
