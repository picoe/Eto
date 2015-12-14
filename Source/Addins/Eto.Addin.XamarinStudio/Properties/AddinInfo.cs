using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin(
	"XamarinStudio", 
	Namespace = "Eto.Addin",
	Version = "2.2.0.0"
)]

[assembly:AddinName("Eto.Forms project templates")]
[assembly:AddinCategory("Eto.Forms")]
[assembly:AddinDescription("Addin to easily start developing with Eto.Forms using project and file templates.")]
[assembly:AddinAuthor("Curtis Wensley")]