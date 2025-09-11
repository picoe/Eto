# PixelLayout

The Pixel Layout provides a way to position your controls based on pixel co-ordinates.

*Note*: Using the pixel layout to position standard controls is highly discouraged, as each control may be different sizes based on the platform. It is highly recommended to use the [[TableLayout]] or [[DynamicLayout]] when using standard controls, as they will auto fit to the size of each control.

```c#
using Eto.Forms;

public class MyForm : Form
{
    public MyForm()
    {
        var layout = new PixelLayout();

        var label = new Label { Text = "Hello!" };
        layout.Add(label, 10, 10);

        // during runtime you can move controls
        layout.Move(label, 10, 100);
        
        Content = layout;
    }
}
```
