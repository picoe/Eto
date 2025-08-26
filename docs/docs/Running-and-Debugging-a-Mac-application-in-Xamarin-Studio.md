To debug your application using a Mac platform in Xamarin Studio on OS X, you need to create a MonoMac (free) or Xamarin.Mac (paid) application project. This has the benefit of creating your .app bundle for you, and if you are using Xamarin.Mac, you can bundle mono inside your application so that your users are not required to install mono.

Follow these steps:

1. Create a new *Empty* Xamarin.Mac or MonoMac application.

  **Note:** XS 6.0 doesn't have an empty Xamarin.Mac project template, so you can just create a new application and delete all files in the project except for Program.cs and Info.plist.

2. Modify the ``Info.plist`` file and set the "Main Interface" setting to blank (also named "Main nib file name" or "NSMainNibFile" if editing as source).
3. Using nuget, add only _one_ of the following packages: 
	* **[Eto.Platform.Mac](https://www.nuget.org/packages/Eto.Platform.Mac/)** (for MonoMac),
	* **[Eto.Platform.XamMac](https://www.nuget.org/packages/Eto.Platform.XamMac/)** (for Xamarin.Mac classic),
	* **[Eto.Platform.XamMac2](https://www.nuget.org/packages/Eto.Platform.XamMac2/)** (for Xamarin.Mac unified).

  Otherwise, add references to **Eto.dll** and one of the following:
	* **Eto.Mac.dll** (for MonoMac),
	* **Eto.XamMac.dll** (for Xamarin.Mac classic),
	* **Eto.XamMac2.dll** (for Xamarin.Mac unified).
4. Add a Program.cs to start your app with the following:

	```C#
	using System;
	using Eto.Forms;
	using Eto.Drawing;
	
	namespace MyApp
	{
		// this should usually be in a shared assembly
		public class MainForm : Form
		{
			public MainForm()
			{
				Title = "Hello, Mac!";
				Menu = new MenuBar(); // to get a standard OS X menu
				ClientSize = new Size(200, 200);
				Content = new Label { Text = "Your content" };
			}
		}
	
		public static class Program
		{
			[STAThread]
			public static void Main(string[] args)
			{

				// note that in Eto.Forms 2.2 auto-detection of the platform does not work for XamMac/XamMac2, so you need to pass the platform ID to the application constructor
				var platform = Eto.Platforms.XamMac2; // or Eto.Platforms.XamMac or Eto.Platforms.Mac

				// MainForm is usually from a shared assembly
				new Application(platform).Run(new MainForm());
			}
		}
	}
	```

Now, you should be able to run and debug your app within Xamarin Studio!