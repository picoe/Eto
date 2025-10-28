# Tutorial 1 Hello Eto.Forms

This tutorial will teach you how to create a simple ["Hello World"](http://en.wikipedia.org/wiki/Hello_world_program) application using Eto.Forms

1. Follow [Quick Start](../Quick-Start.md) to create your solution

2. Create a new class for your main form:

	```c#
	class MyForm : Eto.Forms.Form
	{
		public MyForm()
		{
			// sets the client (inner) size of the window for your content
			this.ClientSize = new Eto.Drawing.Size(600, 400);

			this.Title = "Hello, Eto.Forms";
		}
	}
	```

3. In your **Main()** method, create an `Application` object and run using your form (if you haven't used one of the project templates already):

	```c#
	using System;
	
	class Startup
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Eto.Forms.Application().Run(new MyForm());
		}
	}
	```

- [C# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/CSharp/Tutorial1/Main.cs)
- [F# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/FSharp/Tutorial1/Program.fs)

Next: [Tutorial 2 Menus and Toolbars](Tutorial-2-Menus-and-Toolbars.md)