#if Mac
using System;
using MonoDevelop.Ide.Gui;
using Eto.Forms;
using MonoDevelop.Ide;
using MonoDevelop.Components.Commands;
using System.Reflection;
using System.IO;
using Eto.Designer;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Navigation;
using System.Linq;
using MonoDevelop.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoDevelop.Components.Mac;
using System.Diagnostics;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Core.Text;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Eto.Addin.MonoDevelop.Editor
{
	public class MacEditorView : EditorView
	{
		public MacEditorView(ViewContent content)
			: base(content)
		{
		}

		protected override MonoDevelop.Components.Control GetNativeControl()
		{
			var editorWidget = EditorWidget;
			editorWidget.ShowAll();
			var editor = new GtkEmbed2(editorWidget);
			var editorEto = editor.ToEto();
			Preview = new PreviewEditorView(editorEto, null, null, GetEditorText);
			var nspreview = XamMac2Helpers.ToNative(Preview, true);
			var nsviewContainer = new NSViewContainer2(nspreview);
			return nsviewContainer;
		}
	}
}
#endif