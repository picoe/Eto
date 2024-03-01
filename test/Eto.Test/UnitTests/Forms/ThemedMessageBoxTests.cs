
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class ThemedMessageBoxTests : MessageBoxTests, IDisposable
{
	Platform themedPlatform;
	IDisposable context;
	public ThemedMessageBoxTests()
	{
		Application.Instance.Invoke(() =>
		{
			var mainForm = Application.Instance.MainForm;
			themedPlatform = (Platform)Activator.CreateInstance(Platform.Instance.GetType());
			themedPlatform.Add<MessageBox.IHandler>(() => new Eto.Forms.ThemedControls.ThemedMessageBoxHandler());
			context = themedPlatform.Context;
			var app = new Application(themedPlatform).Attach();
			app.MainForm = mainForm;
		});
	}

	public void Dispose()
	{
		Application.Instance.Invoke(() =>
		{
			context?.Dispose();
			context = null;
		});
	}
}

