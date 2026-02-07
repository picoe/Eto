# DynamicLayout

The DynamicLayout is the most flexible layout for Eto.Forms.  Internally, it uses [TableLayout](./TableLayout.md) Panels to create its content. Unlike other layouts, the DynamicLayout can only be modified during creation.  Once generated, the controls cannot be moved or re-positioned like a [TableLayout](./TableLayout.md) or [PixelLayout](./PixelLayout.md)

Controls in a dynamic layout can be positioned vertically and horizontally.  Each vertical set of controls can be aligned with controls in previous horizontal sections, giving a very easy way to build forms.

## Example

This example creates a top section with two columns (one for the field label, and the other for the data entry), and a second section that has three columns (a spacer and two buttons aligned to the right).

```C#
var layout = new DynamicLayout();

layout.BeginVertical (); // create a fields section
layout.BeginHorizontal ();
layout.Add (new Label { Text = "Field 1" });
layout.Add (new TextBox (), true); // true == scale horizontally
layout.EndHorizontal ();

layout.BeginHorizontal ();
layout.Add (new Label { Text = "Field 2" });
layout.Add (new ComboBox (), true);
layout.EndHorizontal ();
layout.EndVertical ();

layout.BeginVertical (); // buttons section
layout.BeginHorizontal ();
layout.Add (null, true); // add a blank space scaled horizontally to fill space
layout.Add (new Button { Text = "Cancel" });
layout.Add (new Button { Text = "Ok" });
layout.EndHorizontal ();
layout.EndVertical ();
```

Or, in short form:

```C#
var layout = new DynamicLayout();

layout.BeginVertical (); // fields section
layout.AddRow (new Label { Text = "Field 1" }, new TextBox ());
layout.AddRow (new Label { Text = "Field 2" }, new ComboBox ());
layout.EndVertical ();

layout.BeginVertical (); // buttons section
// passing null in AddRow () creates a scaled column
layout.AddRow (null, new Button { Text = "Cancel" }, new Button { Text = "Ok" });
layout.EndVertical ();
```

Or a XAML example:

```xml
<DynamicLayout Spacing="10,5">
    <DynamicRow>
        <Label VerticalAlignment="Center">Server url:</Label>
        <TextBox PlaceholderText="https://192.168.1.101:8443" />
    </DynamicRow>
    <DynamicRow>
        <Label VerticalAlignment="Center">Username:</Label>
        <TextBox PlaceholderText="myusername" />
    </DynamicRow>
    <DynamicRow>
        <Label VerticalAlignment="Center">Password:</Label>
        <PasswordBox Height="25" />
    </DynamicRow>
    <DynamicRow>
        <Label VerticalAlignment="Center">Client certificate fingerprint:</Label>
        <TextBox PlaceholderText="c94955f0e345b1e1248aa97abf01ec0e8f0f9dd7" />
    </DynamicRow>
    <DynamicRow>
        <CheckBox Text="Use values next time" />
    </DynamicRow>
</DynamicLayout>
```
