using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace Eto.Addin.VisualStudio.Intellisense
{
	[Order(Before = "High")]
	[Export(typeof(ICompletionSourceProvider)), ContentType("xeto"), Name("XetoCompletion")]
	public class XamlCompletionProvider : ICompletionSourceProvider
	{
		[Import]
		public IGlyphService GlyphService { get; set; }

		[Import]
		internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

		public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
		{
			return new XamlCompletionSource(this, textBuffer);
		}
	}
}
