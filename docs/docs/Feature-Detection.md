# Feature Detection

Some features of Eto may only be supported on certain platforms.  In this case, feature detection should be used to determine if it is supported by the platform before using it.  This is especially true for mobile vs. desktop, where certain controls may or may not be present, or certain functions of controls.

### Control support


To determine if a control is supported, use `Platform.Supports<T>()` with the corresponding control.  For example:

```C#
using Eto.Forms;

public class MyForm : Form
{
 public MyForm()
 {
  if (Platform.Supports<MenuBar>())
  {
   var menu = new MenuBar();
   // create menu
   myForm.Menu = menu;
  }
 }
}
```

### Feature support

To determine if a control supports a certain feature (property, method, or event), then it should provide a `FeatureIsSupported()` method, where Feature is the corresponding name of the property, method, or event that it references.

Note that some controls may treat certain properties as hints.  For example, the `Dialog.DisplayMode` property can be used to attach a dialog to a parent window on platforms that support it.  Otherwise, it will display in a manner that is appropriate for the platform.