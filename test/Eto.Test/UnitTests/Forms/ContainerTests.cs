using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ContainerTests : TestBase
	{
		[TestCaseSource(nameof(GetPanelTypes)), ManualTest]
		public void PanelPaddingShouldWork(Type panelType)
		{
			ManualForm(
				"There should be 40px padding around the blue rectangle",
				form =>
				{
					var panel = CreatePanelType(panelType, out var container);
					Assert.IsNotNull(panel);

					panel.Padding = 40;
					panel.Content = new Panel
					{
						BackgroundColor = Colors.Blue,
						Size = new Size(200, 200)
					};
					return container;
				});
		}

		[TestCaseSource(nameof(GetPanelTypes)), ManualTest]
		public void PanelPaddingBottomRightShouldWork(Type panelType)
		{
			ManualForm(
				"There should be 40px padding at the bottom and right of the blue rectangle",
				form =>
				{
					var panel = CreatePanelType(panelType, out var container);
					Assert.IsNotNull(panel);

					panel.Padding = new Padding(0, 0, 40, 40);
					panel.Content = new Panel
					{
						BackgroundColor = Colors.Blue,
						Size = new Size(200, 200)
					};
					return container;
				});
		}
	}
}
