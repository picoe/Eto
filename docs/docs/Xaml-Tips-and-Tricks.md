Useful hints when using Eto's Xaml (.xeto) for defining your UI


## Using images or other resources:

When you want to use images in your xaml (.xeto) or other embedded resources, you can use the [ResourceExtension](https://github.com/picoe/Eto/blob/develop/src/Eto.Serialization.Xaml/Extensions/ResourceExtension.cs) markup extension.

1. Add your image(s) to the project
2. Set them as **embedded resource**
3. In the .xeto file, use the `{Resource ...}` markup extension:
```xml
<ImageView Image="{Resource MyAssembly.Path.To.Image.png, MyAssembly}" />
```
The resource file path should be provided in the same format returned by ```Assembly.GetManifestResourceNames()```.

Note: You can omit the assembly at the end if your code behind class is in the same assembly.