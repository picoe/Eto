Most desktop platforms have similar features and functionality with menus and toolbars, with a few exceptions:

* Mac OS X applications typically only have a single toolbar
* Mac OS X applications require certain menu items in your app for standard actions to work, such as Cut/Copy/Paste/Undo/Redo/etc.

Eto.Forms makes dealing with these exceptions very easy.

## Command vs. ToolItem & MenuItem

In Eto.Forms, you can create your toolbars and menus directly using the ``ToolItem`` and ``MenuItem`` subclasses. However, you can also use [Commands](https://github.com/picoe/Eto/blob/develop/src/Eto/Forms/Command.cs) to create them instead. This has a few advantages:

* Command code is separated from menu or toolbar button UI
* You can share the same command between the menu and toolbar

## MenuItem types

* ``ButtonMenuItem`` - Click to trigger an event
* ``CheckMenuItem`` - Toggle on or off
* ``RadioMenuItem`` - Select from a group of options, supports multiple groups
* ``SeparatorMenuItem`` - Logically group your menu items for better readability

## ToolItem types

* ``ButtonToolItem`` - Click to trigger an event
* ``CheckToolItem`` - Toggle on or off
* ``RadioToolItem`` - Select from a group of options, supports only a *single group* in the toolbar.
* ``SeparatorToolItem`` - Logically group your tool items for better readability with space or a divider

## Command types

* ``Command`` - Click to trigger an event
* ``CheckCommand`` - Toggle on or off
* ``RadioCommand`` - Select from a group of options

## Adding a MenuBar and ToolBar to your form

In this tutorial, we will start from [[Tutorial 1 Hello Eto.Forms]].

1. Create a new custom command to share between the menu and toolbar
		
	```c#
	public class MyCommand : Command
	{
		public MyCommand()
		{
			MenuText = "C&lick Me, Command";
			ToolBarText = "Click Me";
			ToolTip = "This shows a dialog for no reason";
			//Image = Icon.FromResource ("MyResourceName.ico");
			//Image = Bitmap.FromResource ("MyResourceName.png");
			Shortcut = Application.Instance.CommonModifier | Keys.M;  // control+M or cmd+M
		}
	
		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
	
			MessageBox.Show(Application.Instance.MainForm, "You clicked me!", "Tutorial 2", MessageBoxButtons.OK);
		}
	}
	```
		
2. Add your command to the File Menu in a MenuBar, and assign it to the form's Menu.

	```c#
	Menu = new MenuBar
	{
		Items =
		{
			new ButtonMenuItem
			{ 
				Text = "&File",
				Items =
				{ 
					// you can add commands or menu items
					new MyCommand(),
					// another menu item, not based off a Command
					new ButtonMenuItem { Text = "Click Me, MenuItem" }
				}
			} 
		}
	}
	```

3. Add a Quit menu item. *Note*  this is optional. On OS X, one will be added for you if you don't specify a Quit item (otherwise there is no easy way for the user to quit your application).

	```c#
	Menu.QuitItem = new Command((sender, e) => Application.Instance.Quit())
	{ 
		MenuText = "Quit", 
		Shortcut = Application.Instance.CommonModifier | Keys.Q
	};
	```

4. Add an About menu item to provide information about your app. This shows how to create a menu item directly, without using a command.

	```c#
	var aboutItem = new ButtonMenuItem { Text = "About..." };
	aboutItem.Click += (sender, e) =>
	{
		var dlg = new Dialog
		{ 
			Content = new Label { Text = "About my app..." }, 
			ClientSize = new Size(200, 200)
		};
		dlg.ShowModal(this);
	};
	Menu.AboutItem = aboutItem;
	```

5. Create a ToolBar

	```c#
	ToolBar = new ToolBar
	{
		Items =
		{ 
			new MyCommand(),
			new SeparatorToolItem(),
			new ButtonToolItem { Text = "Click Me, ToolItem" }
		}
	};
	```

- [C# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/CSharp/Tutorial2/Main.cs)
- [F# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/FSharp/Tutorial2/Program.fs)

Next: [[Tutorial 3 Table Layout]]