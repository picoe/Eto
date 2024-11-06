using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class FloatingFormTests : WindowTests<FloatingForm>
{
	protected override void Test(Action<FloatingForm> test, int timeout = 4000) => Form(test, timeout);
	protected override void ManualTest(string message, Func<FloatingForm, Control> test)
	{
		// ManualForm(message, test);
	}
	protected override void Show(FloatingForm window) => window.Show();
	protected override Task ShowAsync(FloatingForm window) => window.ShowAsync();

}