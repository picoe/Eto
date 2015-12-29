using Eto.Designer.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using mvli = Microsoft.VisualStudio.Language.Intellisense;

namespace Eto.Addin.VisualStudio.Intellisense
{
	class XamlCompletionSource : ICompletionSource
	{
		ITextBuffer buffer;
		XamlCompletionProvider provider;

		static XamlCompletionSource()
		{
			XmlComments.EncodeHtml = false;
        }

		public XamlCompletionSource(XamlCompletionProvider provider, ITextBuffer buffer)
		{
			this.buffer = buffer;
			this.provider = provider;
		}

		/// <summary>
		/// Finds the span of completion text to replace and use for searching.
		/// </summary>
		ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session, CompletionMode mode)
		{
			var end = session.TextView.Caret.Position.BufferPosition;
			var start = end;
			var ch = start.GetChar();
            while (start.Position > 0)
			{
				var temp = start - 1;
				ch = temp.GetChar();
				if (!(char.IsLetterOrDigit(ch) || ch == ':' || ch == '_') || ch == '"' || ch == '\'' || ch == '.')
					break;
				start = temp;
			}
			var span = Span.FromBounds(start.Position, end.Position);
			return start.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);
		}

		public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
		{
			// read XML up to the cursor
			var point = session.TextView.Caret.Position.BufferPosition;
			var info = XmlParser.Read(buffer.CurrentSnapshot.GetText(0, point.Position));
			if (info.Mode == CompletionMode.Class)
			{
				var prevPoint = point - 1;
				var prevCh = prevPoint.GetChar();
                if (prevCh != '<' && prevCh != '.')
				{
					session.Dismiss();
					return;
				}
			}
			var nodes = info.Nodes;
			var ns = nodes.SelectMany(r => r.Namespaces ?? Enumerable.Empty<CompletionNamespace>());
			var path = nodes.Where(r => r.Mode == CompletionMode.Class).Select(r => r.Name).ToList();
			var last = nodes.LastOrDefault();

			// get available completion items
			var items = Designer.Completion.Completion.GetCompletionItems(ns, info.Mode, path, last);

			// translate to VS completions
			var completionList = new List<mvli.Completion>();
            foreach (var cls in items.OrderBy(r => r.Name))
			{
                completionList.Add(new mvli.Completion(cls.Name, cls.Name, cls.Description, GetGlyph(cls.Type), null));
			}

			completionSets.Insert(0, new CompletionSet(
				"Eto",
				"Eto",
				FindTokenSpanAtPosition(session.GetTriggerPoint(buffer), session, info.Mode),
				completionList,
				null));
			return;
		}

		System.Windows.Media.ImageSource GetGlyph(CompletionType type)
		{
			var service = provider.GlyphService;
            switch (type)
			{
				case CompletionType.Class:
					return service.GetGlyph(StandardGlyphGroup.GlyphGroupClass, StandardGlyphItem.GlyphItemPublic);
				case CompletionType.Property:
					return service.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.GlyphItemPublic);
				case CompletionType.Event:
					return service.GetGlyph(StandardGlyphGroup.GlyphGroupEvent, StandardGlyphItem.GlyphItemPublic);
				case CompletionType.Field:
					return service.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic);
				case CompletionType.Attribute:
					return service.GetGlyph(StandardGlyphGroup.GlyphXmlAttribute, StandardGlyphItem.GlyphItemPublic);
				default:
				case CompletionType.Literal:
					return service.GetGlyph(StandardGlyphGroup.GlyphGroupConstant, StandardGlyphItem.GlyphItemPublic);
			}
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}
