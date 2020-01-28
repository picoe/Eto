using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Eto.Addin.VisualStudio
{
	internal static class Components
	{
#pragma warning disable 649
		// Use XML editor for xeto files
		[Export]
		[Name("xeto")]
		[BaseDefinition("XML")]
		internal static ContentTypeDefinition XetoContentTypeDefinition;

		[Export]
		[FileExtension(".xeto")]
		[ContentType("xeto")]
		internal static FileExtensionToContentTypeDefinition XetoExtensionDefinition;


		// Use Json editor for jeto files
		[Export]
		[FileExtension(".jeto")]
		[ContentType("Json")]
		internal static FileExtensionToContentTypeDefinition JetoFileExtensionDefinition;
#pragma warning restore 649
	}
}
