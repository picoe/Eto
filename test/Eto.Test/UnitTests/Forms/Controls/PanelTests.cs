using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				Assert.AreSame(panel1, label.Parent, "#1");
				Assert.AreSame(panel1, label.VisualParent, "#2");

				panel1.Content = null;
				Assert.AreSame(null, label.Parent, "#2");
				Assert.AreSame(null, label.VisualParent, "#3");

				panel2.Content = label;
				Assert.AreSame(panel2, label.Parent, "#3");
				Assert.AreSame(panel2, label.VisualParent, "#4");
			});
		}
	}
}
