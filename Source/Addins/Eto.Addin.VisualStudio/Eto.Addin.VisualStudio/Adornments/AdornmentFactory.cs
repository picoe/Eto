using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Eto.Addin.VisualStudio.Util;
using System.Linq;

namespace Eto.Addin.VisualStudio.Adornments
{
	/// <summary>
	/// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
	/// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
	/// </summary>
	[Export(typeof(IWpfTextViewCreationListener))]
	[ContentType("text")]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class AdornmentFactory : IWpfTextViewCreationListener
	{
		/// <summary>
		/// Defines the adornment layer for the adornment. This layer is ordered 
		/// after the selection layer in the Z-order
		/// </summary>
		[Export(typeof(AdornmentLayerDefinition))]
		[Name("Eto.ColorAdornment")]
		[Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
		public AdornmentLayerDefinition editorAdornmentLayer = null;

		/// <summary>
		/// Instantiates a TextAdornment1 manager when a textView is created.
		/// </summary>
		/// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
		public void TextViewCreated(IWpfTextView textView)
		{
			// only CSharp files
			if (textView.TextBuffer.ContentType.TypeName != "CSharp")
				return;

			// if it's part of a project, it must be referencing Eto.dll.
			var project = textView.GetContainingProject();
			if (project != null)
			{
				var vsproject = project.Object as VSLangProj.VSProject;
				if (vsproject != null)
				{
					var references = vsproject.References.OfType<VSLangProj.Reference>().ToList();
					if (!references.Any(r => r.Name == "Eto"))
						return;
                }
			}

			new ColorAdornment(textView);
		}
	}
}
