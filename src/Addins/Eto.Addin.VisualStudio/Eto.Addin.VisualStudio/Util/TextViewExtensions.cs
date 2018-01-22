using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Util
{
	static class TextViewExtensions
	{
		public static string GetFilePath(this IWpfTextView wpfTextView)
		{
			Microsoft.VisualStudio.Text.ITextDocument document;
			if ((wpfTextView == null) ||
					(!wpfTextView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document)))
				return String.Empty;

			// If we have no document, just ignore it.
			if ((document == null) || (document.TextBuffer == null))
				return String.Empty;

			return document.FilePath;
		}

		public static Project GetContainingProject(this IWpfTextView wpfTextView)
		{
			return GetContainingProject(GetFilePath(wpfTextView));
		}

		public static Project GetContainingProject(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				var dte2 = (DTE2)Package.GetGlobalService(typeof(SDTE));
				if (dte2 != null)
				{
					var prjItem = dte2.Solution.FindProjectItem(fileName);
					if (prjItem != null)
						return prjItem.ContainingProject;
				}
			}
			return null;
		}
	}
}
