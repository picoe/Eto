Container controls act as a control and can be added as a child to any other container. The layout controls supported are:

- [Panel](https://github.com/picoe/Eto/blob/develop/src/Eto/Forms/Controls/Panel.cs) - Controls that subclass ``Panel``, such as ``Window``, ``GroupBox``, ``Scrollable``, etc allow you to specify a single child Content control
- [[TableLayout]] - Similar to how an HTML table works, with a single control per cell
- [[PixelLayout]] - Specify X,Y co-ordinates for the position of each control (from Upper-Left)
- [[DynamicLayout]] - Dynamic hierarchical horizontal and vertical layout
- [[StackLayout]] - Horizontal or Vertical list of controls

Containers are implemented so that you do *not* need to specify a size for the container (although you can if you want).  This allows containers to automatically size to fit the child controls, which works nicely across different platforms.

For example, if you created a ``Dialog`` or ``Form`` and didn't specify any sizes for any controls, it should automatically size to fit its contents.
