Eto.Forms comes with many standard controls, however sometimes you may want to make full use of each platform's abilities, or if you want to re-use platform specific code already written. Custom platform controls can be created for this purpose.

Custom platform controls can have implementations for each platform, and are implemented exactly the way standard Eto.Forms controls are. You can either extend an existing control and provide a custom handler for each platform, or create a completely custom control.

All supportable Eto.Forms control handlers are registered through the ``Platform`` object.  When creating a new Eto.Forms control (custom or not), the ``Platform`` looks up the control handler via its interface type specified in the ``HandlerAttribute``.

The drawback of creating custom platform controls is that you have to create a handler implementation of the control for each platform you wish to use it with.


## Example

This is an example of creating a custom text box control.

In your cross-platform library:
	
```C#
// control to use in your eto.forms code
[Handler(typeof(IMyCustomControl))]
public class MyCustomControl : Control
{
	IMyCustomControl Handler { get { return (IMyCustomControl)base.Handler; } }

	public string MyProperty
	{
		get { return Handler.MyProperty; }
		set { Handler.MyProperty = value; }
	}
	
	// interface to the platform implementations
	public new interface IMyCustomControl : Control.IHandler
	{
		string MyProperty { get; set; }
	}
}
```

	
In your MonoMac platform assembly:

```C#
public class MyCustomControlHandler : MacControl<NSTextField, MyCustomControl, MyCustomControl.ICallback>, MyCustomControl.IMyCustomControl
{
	public MyCustomControlHandler ()
	{
		this.Control = new NSTextField ();
	}
	
	public string MyProperty
	{
		get { return this.Control.StringValue; }
		set { this.Control.StringValue = value ?? string.Empty; }
	}
}
```

In your WPF assembly:

```C#
public class MyCustomControlHandler : WpfControl<System.Windows.Controls.TextBox, MyCustomControl, MyCustomControl.ICallback>, MyCustomControl.IMyCustomControl
{
	public MyCustomControlHandler ()
	{
		this.Control = new System.Windows.Controls.TextBox ();
	}
	
	public string MyProperty
	{
		get { return this.Control.Text; }
		set { this.Control.Text = value; }
	}
}
```

To register your new control, you must add it to the platform instance.  This can be done *before* creating your application in your main() method.

```C#
var platform = new Eto.Mac.Platform(); // mac platform

// to register your custom control handler, call this before using your class:
platform.Add<MyCustomControl.IMyCustomControl>(() => new MyCustomControlHandler());

// create application object, and run
new Application(platform).Run(
	new Form
	{
		Title = "Hello!",
		Content = new MyCustomControl { MyProperty = "There" }
	}		
);
```