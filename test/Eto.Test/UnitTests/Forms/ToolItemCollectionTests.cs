using System;
using NUnit.Framework;
using Eto.Forms;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ToolItemCollectionTests
	{
		[Test]
		public void MixedCommandsAndToolItemsShouldBeInCorrectOrder()
		{
			TestBase.Invoke(() =>
			{
				var toolbar = new ToolBar();
				for (int i = 0; i < 15; i++)
				{
					switch (i % 5)
					{
						case 0:
							toolbar.Items.Add(new ButtonToolItem { Text = i.ToString() });
							break;
						case 1:
							toolbar.Items.Add(new ButtonToolItem { Text = i.ToString() });
							break;
						case 2:
							toolbar.Items.Add(new Command { ToolBarText = i.ToString() });
							break;
						case 3:
							toolbar.Items.AddSeparator();
							break;
						case 4:
							// convert to toolitem first
							ToolItem toolItem = new Command { ToolBarText = i.ToString() };
							toolbar.Items.Add(toolItem);
							break;
					}
				}
				for (int i = 0; i < toolbar.Items.Count; i++)
				{
					if (toolbar.Items[i] is SeparatorToolItem)
						continue;
					Assert.AreEqual(i.ToString(), toolbar.Items[i].Text, "Items are out of order");
				}
			});
		}
	}
}

