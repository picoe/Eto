using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin(
	"MonoDevelop", 
	Namespace = "Eto.Addin",
	Version = "2.4.9999.0"
)]

[assembly:AddinName("Eto.Forms Support Addin")]
[assembly:AddinCategory("Eto.Forms")]
[assembly:AddinDescription(@"Addin to easily start developing with the Eto.Forms cross platform UI framework.

Provides:

- File and Project templates for C#, VB.NET, and F#.
- Autocomplete for the Xaml view editor.
- Xaml, Json, and Code based live form preview.")]
[assembly:AddinAuthor("Curtis Wensley")]