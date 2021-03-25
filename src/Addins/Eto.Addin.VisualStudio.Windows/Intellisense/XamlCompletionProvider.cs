using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;

namespace Eto.Addin.VisualStudio.Intellisense
{
	[Order(Before = "XML")]
	[ContentType("xeto"), Name("XetoCompletion")]
	[Export(typeof(IAsyncCompletionSourceProvider))]
	public class XamlCompletionSourceProvider : IAsyncCompletionSourceProvider
	{
		public IAsyncCompletionSource GetOrCreate(ITextView textView) => new XamlCompletionSource(textView);
	}

	[Order(Before = "XML")]
	[ContentType("xeto"), Name("XetoCompletion")]
	[Export(typeof(IAsyncCompletionCommitManagerProvider))]
	public class XamlCompletionManagerProvider : IAsyncCompletionCommitManagerProvider
	{
		public IAsyncCompletionCommitManager GetOrCreate(ITextView textView) => new XamlCompletionManager(textView);
	}
}
