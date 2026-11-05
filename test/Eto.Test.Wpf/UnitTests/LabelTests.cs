using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using swd = System.Windows.Documents;
namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class LabelTests : TestBase
	{
		[Test]
		public void LabelShouldPreserveMnemonicTextAndUnderline() => Invoke(() =>
		{
			using var label = new Label { Text = "Open &File" };
			var native = (EtoAccessLabel)label.ControlObject;
			var accessText = (swc.AccessText)native.Content;
			Assert.That(accessText.Text, Is.EqualTo("Open _File"));
			Assert.That(accessText.AccessKey, Is.EqualTo('F'));

			native.EnableMnemonic = false;
			native.AlwaysShowMnemonic = true;
			var textBlock = (swc.TextBlock)native.Content;
			Assert.That(new swd.TextRange(textBlock.ContentStart, textBlock.ContentEnd).Text, Is.EqualTo("Open File"));
			var underline = textBlock.Inlines.OfType<swd.Underline>().Single();
			Assert.That(((swd.Run)underline.Inlines.Single()).Text, Is.EqualTo("F"));
		});

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
