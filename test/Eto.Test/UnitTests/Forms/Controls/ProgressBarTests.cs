using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ProgressBarTests : TestBase
	{
		[Test]
		public void IncrementingByOneShouldGetToTheEnd()
		{
			Invoke(() =>
			{
				var progressBar = new ProgressBar();
				progressBar.MaxValue = 100;
				progressBar.MinValue = 0;
				progressBar.Value = 0;
				for (int i = 0; i < 100; i++)
				{
					progressBar.Value++;
				}	
				Assert.That(progressBar.Value, Is.EqualTo(100), "#1");
			});
		}
	}
}
