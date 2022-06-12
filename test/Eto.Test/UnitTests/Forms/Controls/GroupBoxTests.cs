using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class GroupBoxTests : TestBase
	{
		[Test]
		public void GroupBoxShouldHaveCorrectlySizedContent()
		{
			GroupBox groupBox = null;
			Shown(form =>
			{
				groupBox = new GroupBox { Content = new Panel { Size = new Size(200, 200) } };
				return TableLayout.AutoSized(groupBox);
			}, c =>
			{
				Assert.AreEqual(new Size(200, 200), groupBox.Content.Size, "#1 Content Size should auto size to its desired size");
			});
		}
	}
}