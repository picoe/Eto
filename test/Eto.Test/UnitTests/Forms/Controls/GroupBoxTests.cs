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
				Assert.That(groupBox.Content.Size, Is.EqualTo(new Size(200, 200)), "#1 Content Size should auto size to its desired size");
			});
		}
	}
}