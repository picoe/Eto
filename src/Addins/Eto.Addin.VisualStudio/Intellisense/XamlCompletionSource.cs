using Eto.Designer.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using mvli = Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Immutable;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Core.Imaging;

namespace Eto.Addin.VisualStudio.Intellisense
{
	class XamlCompletionSource : IAsyncCompletionSource
	{
		ITextView textView;

		static XamlCompletionSource()
		{
			XmlComments.EncodeHtml = false;
        }

		public XamlCompletionSource(ITextView textView)
		{
			this.textView = textView;
		}

		ImageElement GetGlyph(CompletionType type)
		{
            switch (type)
			{
				case CompletionType.Class:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Class));
				case CompletionType.Property:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Property));
				case CompletionType.Event:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Event));
				case CompletionType.Field:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Field));
				case CompletionType.Attribute:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Attribute));
				default:
				case CompletionType.Literal:
					return new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.Literal));
			}
		}

		public void Dispose()
		{
		}

		public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
		{
			//session.Properties["LineNumber"] = triggerLocation.GetContainingLine().LineNumber;
			var point = applicableToSpan.Start;
			var text = textView.TextBuffer.CurrentSnapshot.GetText(0, point.Position);
			return Task.Run(() =>
			{
				try
				{
					// read XML up to the cursor
					var info = XmlParser.Read(text);
					if (info.Mode == CompletionMode.Class)
					{
						var prevPoint = point - 1;
						var prevCh = prevPoint.GetChar();
						if (prevCh != '<' && prevCh != '.')
						{
							return CompletionContext.Empty;
						}
					}

					var nodes = info.Nodes;
					var ns = nodes.SelectMany(r => r.Namespaces ?? Enumerable.Empty<CompletionNamespace>());
					var path = nodes.Where(r => r.Mode == CompletionMode.Class).Select(r => r.Name).ToList();
					var last = nodes.LastOrDefault();

					// get available completion items
					var items = Designer.Completion.Completion.GetCompletionItems(ns, info.Mode, path, last);

					// translate to VS completions
					var completionList = new List<mvli.AsyncCompletion.Data.CompletionItem>();
					foreach (var cls in items.OrderBy(r => r.Name))
					{
						var displayText = cls.Name;
						var item = new mvli.AsyncCompletion.Data.CompletionItem(displayText,
							source: this,
							filters: ImmutableArray<CompletionFilter>.Empty,
							icon: GetGlyph(cls.Type),
							suffix: cls.Suffix ?? string.Empty,
							attributeIcons: ImmutableArray<ImageElement>.Empty,
							insertText: displayText, sortText: displayText, filterText: displayText
							);
						item.Properties["eto"] = cls;
						completionList.Add(item);
					}

					return new CompletionContext(completionList.ToImmutableArray(), null, InitialSelectionHint.RegularSelection);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Error doing autocomplete: {ex}");
					throw;
				}
			});
		}

		public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, mvli.AsyncCompletion.Data.CompletionItem item, CancellationToken token)
		{
			var etoitem = item.Properties["eto"] as Designer.Completion.CompletionItem;
			return Task.FromResult<object>(etoitem?.Description);
		}

		public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
		{
			if ((
					trigger.Reason == CompletionTriggerReason.Insertion 
					|| trigger.Reason == CompletionTriggerReason.Invoke
				)
				&& !char.IsControl(trigger.Character))
			{
				var span = XamlCompletionManager.FindTokenSpan(new SnapshotSpan(triggerLocation, triggerLocation));

				return new CompletionStartData(CompletionParticipation.ProvidesItems, span);
			}

			return CompletionStartData.DoesNotParticipateInCompletion;
		}
	}
}
