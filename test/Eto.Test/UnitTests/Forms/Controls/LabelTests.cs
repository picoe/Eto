using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class LabelTests : TestBase
	{
		[Test, ManualTest]
		public void ChangingTextAfterCreationShouldUpdateSize()
		{
			ManualForm("Label should be fully visible", form =>
			{
				var label = new Label { Text = "" };
				Application.Instance.AsyncInvoke(() => label.Text = "This label should end with a period.");
				var layout = new PixelLayout
				{
					Size = new Size(300, 200)
				};
				layout.Add(label, 0, 0);
				return layout;
			});
		}

		[Test, ManualTest]
		public void WrapModeNoneShouldAllowNewLines()
		{
			ManualForm("Label should have two lines of text", form =>
			{
				var label = new Label
				{
					Text = Utility.LoremTextWithTwoParagraphs,
					Wrap = WrapMode.None
				};
				var layout = new PixelLayout
				{
					Size = new Size(300, 200)
				};
				layout.Add(label, 0, 0);
				return layout;
			});
		}
	}
}
