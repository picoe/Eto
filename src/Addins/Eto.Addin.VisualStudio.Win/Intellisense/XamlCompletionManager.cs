using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

namespace Eto.Addin.VisualStudio.Intellisense
{
	public class XamlCompletionManager : IAsyncCompletionCommitManager
	{
		ITextView textView;
		public IEnumerable<char> PotentialCommitCharacters { get; }

		public XamlCompletionManager(ITextView textView)
		{
			PotentialCommitCharacters = new[] { '\n', '/', '>', '.', '=', ' ', '"', '\'' };
			this.textView = textView;
		}

		public static SnapshotSpan FindTokenSpan(SnapshotSpan applicableToSpan)
		{
			var end = applicableToSpan.End;
			var start = applicableToSpan.Start;
			char ch;
			if (start != end)
			{
				ch = start.GetChar();
				if (ch == '"' || ch == '\'')
				{
					start += 1;
				}
				ch = (end - 1).GetChar();
				if (ch == '"' || ch == '\'')
				{
					end -= 1;
				}
			}
			
			while (start.Position > 0)
			{
				var temp = start - 1;
				ch = temp.GetChar();
				if (!(char.IsLetterOrDigit(ch) || ch == ':' || ch == '_') || ch == '"' || ch == '\'' || ch == '.')
					break;
				start = temp;
			}
			return new SnapshotSpan(start, end);
		}

		public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token)
		{
			return true;
		}

		public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
		{
			var etoitem = item.Properties["eto"] as Designer.Completion.CompletionItem;

			// only complete on '.' for child properties.
			if (typedChar == '.' && !etoitem.Behavior.HasFlag(Designer.Completion.CompletionBehavior.ChildProperty))
				return CommitResult.Unhandled;

			var span = session.ApplicableToSpan.GetSpan(buffer.CurrentSnapshot);
			span = FindTokenSpan(span);
			var text = item.InsertText;
			var result = CommitResult.Handled;
			switch (typedChar)
			{
				case '.':
					if (!text.EndsWith("."))
						text += ".";
					result = new CommitResult(true, CommitBehavior.CancelCommit | CommitBehavior.SuppressFurtherTypeCharCommandHandlers);
					break;
				case '/':
					text += " />";
					result = new CommitResult(true, CommitBehavior.SuppressFurtherTypeCharCommandHandlers);
					break;
				case '\n':
					result = new CommitResult(true, CommitBehavior.RaiseFurtherReturnKeyAndTabKeyCommandHandlers);
					break;
			}
			var newSnapshot = buffer.Replace(span, text);
			var endLocation = new SnapshotPoint(newSnapshot, span.End.Position + text.Length - span.Length);
			switch (typedChar)
			{
				case '.':
					session.OpenOrUpdate(new CompletionTrigger(CompletionTriggerReason.Insertion, newSnapshot, '.'), endLocation, token);
					break;
			}
			var ch = endLocation.GetChar();
			if (ch == '\'' || ch == '"')
			{
				endLocation += 1;
				textView.Caret.MoveTo(endLocation);
			}
			return result;
		}
	}
}
