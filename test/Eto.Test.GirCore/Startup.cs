using Eto;
using Eto.Test;
namespace Eto.Test.GirCore;

class Startup
{
	[STAThread]
	static void Main(string[] args)
	{
#if DEBUG
		HotReloadService.Initialize();
#endif
		var platform = new Eto.GirCore.Platform();
		// platform.Add<INativeHostControls>(() => new NativeHostControls());
		var app = new Application(platform);
		app.Initialized += (sender, e) =>
		{
			var window = new Form { Title = "Woot", Size = new Size(400, 300) };
			window.Content = new Label { Text = "Hello, GirCore!", VerticalAlignment = VerticalAlignment.Center };
			window.Show();
		};
		// var app = new TestApplication(platform);
		// app.TestAssemblies.Add(typeof(Startup).Assembly);
		app.Run();
	}
}

