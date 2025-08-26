This shows how you can create a new project manually.

Creating a new application
--------------------------

1. Create a new **console** application in Visual Studio or MonoDevelop.

	*Note:* Creating a console application is recommended as it'll add the least amount of extraneous dependencies that you do not need.

2. Change the project type to **Executable with GUI**
	
	### VS for Mac / MonoDevelop
	
	1. Right click on the project, select Options
	2. Go to Build > General
	3. Change *Compile Target* to "Executable with GUI"
	
	### Visual Studio
	
	1. Right click on the project, select Properties
	2. In the Application pane, set *Output Type* to "Windows Application"

3. Add the [Eto.Forms](https://www.nuget.org/packages/Eto.Forms/) nuget package to your project

	*Note:* Remove any references to other GUI frameworks, e.g. *System.Windows.Forms* or *System.Drawing*.

4. Add one or more of the platform nuget packages:

	These will allow your application to run using the native toolkits.  Note that you should only add a single *Mac* package.

	- [Eto.Platform.Gtk](https://www.nuget.org/packages/Eto.Platform.Gtk/)
	- [Eto.Platform.Gtk2](https://www.nuget.org/packages/Eto.Platform.Gtk2/)
	- [Eto.Platform.Gtk3](https://www.nuget.org/packages/Eto.Platform.Gtk3/)
	- [Eto.Platform.Mac](https://www.nuget.org/packages/Eto.Platform.Mac/)
	- [Eto.Platform.Mac64](https://www.nuget.org/packages/Eto.Platform.Mac64/)
	- [Eto.Platform.Windows](https://www.nuget.org/packages/Eto.Platform.Windows/)
	- [Eto.Platform.Wpf](https://www.nuget.org/packages/Eto.Platform.Wpf/)
	- [Eto.Platform.XamMac2](https://www.nuget.org/packages/Eto.Platform.XamMac2/)

Creating a library
------------------

Using a library to house all of your UI code is recommended, so you can separate platform-specific code from your UI.

1. Create a new library in Visual Studio or MonoDevelop

2. Add the [Eto.Forms](https://www.nuget.org/packages/Eto.Forms/) nuget package to your project

3. Reference your UI library from each of your application projects

Next Steps: [[Tutorial 1 Hello Eto.Forms]]