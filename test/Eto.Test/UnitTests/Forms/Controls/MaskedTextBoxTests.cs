using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls;

[TestFixture]
public class MaskedTextBoxTests : TestBase
{
	/// <summary>
	/// Typing is handled by the mask rather than the native control, so the caret the mask chose has
	/// to survive the platform finishing its own insert (#3188).
	/// </summary>
	[Test]
	public void TypingShouldAdvanceCaretPastLiterals()
	{
		TestInput.EnsureSupported();

		MaskedTextBox textBox = null;
		ShownAsync(form =>
		{
			textBox = new MaskedTextBox(new FixedMaskedTextProvider("(999) 000-0000"));
			form.Content = textBox;
		}, async () =>
		{
			var form = (Window)textBox.ParentWindow;
			textBox.Focus();
			await Task.Delay(100);
			Assert.That(textBox.HasFocus, Is.True, "#1 Control should have focus");

			foreach (var key in new[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4 })
			{
				await TestInput.SendKeyDownAsync(form, key);
				await TestInput.SendKeyUpAsync(form, key);
			}

			Assert.That(textBox.Text, Does.StartWith("(123) 4"), "#2 Text should be placed by the mask");
			Assert.That(textBox.CaretIndex, Is.EqualTo(7), "#3 Caret should be after the last typed character");
		});
	}
}
