
namespace Eto.Test.iOS
{
	public class Startup
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			var platform = new Eto.iOS.Platform();
			var app = new TestApplication(platform);
			app.Run();
		}
	}
}
