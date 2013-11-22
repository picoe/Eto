
namespace Eto.Test.iOS
{
	public class Startup
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			var generator = new Eto.Platform.iOS.Generator();
			var app = new TestApplication(generator);
			app.Run(args);
		}
	}
}
