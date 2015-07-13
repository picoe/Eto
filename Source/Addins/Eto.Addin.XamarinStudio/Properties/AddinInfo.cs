using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin(
	"XamarinStudio", 
	Namespace = "Eto.Addin",
	Version = "1.0.2"
)]

[assembly:AddinName("Eto.Forms project templates")]
[assembly:AddinCategory("Eto.Forms")]
[assembly:AddinDescription("Addin to easily start developing with Eto.Forms using project and file templates.")]
[assembly:AddinAuthor("Curtis Wensley")]

//[assembly:AddinDependency("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
//[assembly:AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
