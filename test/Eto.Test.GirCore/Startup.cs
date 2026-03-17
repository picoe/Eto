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
		// var app = new Application(platform);
		// app.Initialized += (sender, e) =>
		// {
		// 	var window = new Form { Title = "Woot", Size = new Size(400, 300) };
		// 	// window.Content = new Label { Text = "Hello, GirCore!", VerticalAlignment = VerticalAlignment.Center };
		// 	window.Content = new TableLayout
		// 	{
		// 		Rows = {
		// 			new Label { Text = "Hello, GirCore!", VerticalAlignment = VerticalAlignment.Center },
		// 			new TextBox { Text = "Hello, GirCore!" },
		// 			new Button { Text = "Click Me" },
		// 			new CheckBox { Text = "Check Me" },
		// 			new RadioButton { Text = "Radio Me" },
		// 			new ListBox { Items = { "Item 1", "Item 2", "Item 3" } },
		// 			null,
		// 		}
		// 	};
		// 	window.Show();
		// };
		var app = new TestApplication(platform);
		app.TestAssemblies.Add(typeof(Startup).Assembly);
		app.Run();
	}
}

