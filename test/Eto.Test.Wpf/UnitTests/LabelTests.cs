using Eto.Test.UnitTests;
using NUnit.Framework;
namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class LabelTests : TestBase
	{
		[ManualTest, Test]
		public void LabelShouldNotConstantlyInvalidateMeasure()
		{
			ManualForm("Set screen resolution to 150% and ensure it doesn't constantly use CPU", form =>
			{
				return new Panel
				{
					Content = new Scrollable
					{
						Border = BorderType.None,
						Content = new TableLayout
						{
							Spacing = new Size(5, 5),
							Rows =
							{
							  new TableRow("A", new Label { Text = "Increment:" }),
							  new TableRow("B", new TextStepper { Width = 70 }, null),
							  null
							}
						}
					}
				};
			});
		}
	}
}
