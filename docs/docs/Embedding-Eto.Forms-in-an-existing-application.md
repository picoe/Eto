Eto.Forms makes it easy to either embed Eto.Forms controls in an existing application or embed native controls within Eto.Forms.  

Helper extension methods are provided in the Eto.Forms namespace such as `myEtoControl.ToNative()` to convert an Eto.Forms control to a native control, and `myNativeControl.ToEto()` to convert a native control to an Eto.Forms control.  These extension methods are available when you reference the platform assembly directly, such as **Eto.WinForms**, **Eto.Mac**, etc.

## Using Eto.Forms controls within a native application

To use Eto.Forms within an existing application, you need to first initialize Eto.Forms after the startup of your app using the following to 'attach' to the existing application (make sure to change the platform to the one you are embedding into):

```C#
new Eto.Forms.Application(Eto.Platforms.WinForms).Attach();
```

You can then create and show Eto.Forms windows and dialogs.

To use an Eto.Forms control as a *child* of a native control, you need to reference the platform assembly you are targeting (e.g **Eto.WinForms.dll**), which provides a Eto.Forms.Control.ToNative() extension that returns the control to use in the native platform:

```C#
using Eto.Forms;

// ...

// create eto control
var etoControl = new Eto.Forms.Panel();

// get native container for the control
System.Windows.Forms.Control myEtoControlAsWinForms = eto.ToNative(true); // true when adding to a native control directly

// add to a native control
var myWinFormsControl = new System.Windows.Forms.Panel();
myEtoControlAsWinForms.Dock = System.Windows.Forms.DockStyle.Fill;
myWinFormsControl.Controls.Add(myEtoControlAsWinForms);
```

## Using native controls within Eto.Forms

To use native controls *within* Eto.Forms, you need to specify the platform you want Eto.Forms to target first.  You can do this by passing the desired platform type to the constructor of the `Application`, like so (make sure to change the platform to the one of your native control):

```C#
new Eto.Forms.Application(Eto.Platforms.WinForms).Run(new MainForm());
```

Now that you have specified what platform Eto.Forms should run in, you can now embed native controls of that platform. Each platform assembly comes with a `ToEto()` extension method in the `Eto.Forms` namespace. For example, to embed a windows forms control in an Eto.Forms application, reference both **Eto.dll** and **Eto.WinForms.dll** (or the platform you are targeting), and do the following:

```C#
using Eto.Forms;

// ...

System.Windows.Forms.Control myWinFormsControl = new MyWinFormsControl();

Eto.Forms.Control myWinFormsControlAsEto = myWinFormsControl.ToEto();

// now you can add myWinFormsControlAsEto to an Eto.Forms layout or container.

```

## Samples

Full samples of how to do both types of integration are provided in the [Samples folder](https://github.com/picoe/Eto/tree/develop/samples).