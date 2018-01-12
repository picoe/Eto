#if blah
using System;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;
namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class ScrollableTests : TestBase
	{
		/// <summary>
		/// When we set a BackgroundColor of a panel, we set WantsLayer and CanDrawSubviewsIntoLayer to true.
		/// This causes problems with a Scrollable as it makes the content duplicate when scrolling as it was drawn
		/// to the parent layer.
		/// </summary>
		[Test, ManualTest]
		public void ScrollableInLayerShouldNotDuplicateContents()
		{
			ManualForm("Scroll the box, the content should not duplicate", form =>
			{
				form.BackgroundColor = Colors.White;
				var panel = new Panel { Size = new Size(200, 200) };
				var button = new Button { Text = "Click Me" };
				var content = new TableLayout
				{
					Rows = {
						button,
						panel
					}
				};

				button.Click += (sender, e) => 
				{
					var scrollContent = new TableLayout();
					for (int i = 0; i < 50; i++)
					{
						scrollContent.Rows.Add(new CheckBox { Text = "Check Box " + i });
					}

					panel.Content = new TableLayout
					{
						Padding = 20,
						Rows = { new Scrollable { Content = scrollContent } }
					};
				};

				return content;
			});
		}
	}
}
#endif