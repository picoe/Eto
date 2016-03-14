using Eto.Addin.VisualStudio.Util;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VSStd2KCmdID = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Eto.Addin.VisualStudio.Intellisense
{
	class XamlCompletionHandler : IOleCommandTarget
	{
		IOleCommandTarget nextCommandHandler;
        ITextView textView;
		TextViewListener listener;

		public XamlCompletionHandler(IVsTextView textViewAdapter, ITextView textView, TextViewListener listener)
		{
			this.textView = textView;
			this.listener = listener;

			//add the command to the command chain
			textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
		}

		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
		}

		static readonly Cmd ReturnCmd = Cmd.FromID(VSStd2KCmdID.RETURN);
		static readonly Cmd TypeCharCmd = Cmd.FromID(VSStd2KCmdID.TYPECHAR);
		static readonly Cmd TabCmd = Cmd.FromID(VSStd2KCmdID.TAB);
		static readonly Cmd CompleteWordCmd = Cmd.FromID(VSStd2KCmdID.COMPLETEWORD);

		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			var cmd = new Cmd(pguidCmdGroup, nCmdID);
			if (VsShellUtilities.IsInAutomationFunction(Services.VsServiceProvider))
			{
				return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
			}
			//make a copy of this so we can look at it after forwarding some commands
			char ch = char.MinValue;
			//make sure the input is a char before getting it
			if (cmd == TypeCharCmd)
			{
				ch = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
			}

			if ((ch == '"' || ch == '\'') && textView.Caret.Position.BufferPosition.GetChar() == ch)
			{
				textView.Caret.MoveToNextCaretPosition();
                return VSConstants.S_OK;
			}

			// check for commit character for current completion
			var session = listener.CompletionBroker.GetSessions(textView).FirstOrDefault(r => !r.IsDismissed);
			if (session != null)
			{
				if (cmd == ReturnCmd
					|| cmd == TabCmd
					|| char.IsWhiteSpace(ch)
					|| ch == '>'
					|| ch == '.'
					|| ch == '=')
				{
                    var selectionStatus = session.SelectedCompletionSet.SelectionStatus;

                    if (selectionStatus.IsSelected)
					{
						Action complete = () =>
						{
							if (session.SelectedCompletionSet == null)
								return;
							var endPoint = session.SelectedCompletionSet.ApplicableTo.GetEndPoint(session.TextView.TextSnapshot);
							var endChar = endPoint.GetChar();
							session.Commit();
							if (endChar == '"' || endChar == '\'')
								session.TextView.Caret.MoveToNextCaretPosition();
                            if (ch == '.'
                                || (cmd == ReturnCmd && selectionStatus.Completion.InsertionText.EndsWith("."))) // property element
                            {
                                TriggerCompletion();
                            }
                        };
						if (cmd == ReturnCmd || cmd == TabCmd || ch == '.')
						{
							Eto.Forms.Application.Instance.AsyncInvoke(complete);
							return VSConstants.S_OK;
						}
						complete();
					}
					else
						session.Dismiss();
				}
				else if (cmd == CompleteWordCmd || ch == '<' || ch == '.')
				{
					session.Dismiss();
				}
			}

			if (cmd == CompleteWordCmd
				|| char.IsLetterOrDigit(ch)
				|| ch == '.'
				|| ch == ' '
				|| ch == '='
			)
			{
				int retVal = VSConstants.S_OK;
				if (cmd != CompleteWordCmd)
					retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
				if (session == null || session.IsDismissed)
					Eto.Forms.Application.Instance.AsyncInvoke(TriggerCompletion);

				return retVal;
			}

			return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
		}

		void TriggerCompletion()
		{
			var session = listener.CompletionBroker.GetSessions(textView).FirstOrDefault(r => !r.IsDismissed);
			if (session != null && !session.IsDismissed)
				return;
			//the caret must be in a non-projection location 
			SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(
				textBuffer => !textBuffer.ContentType.IsOfType("projection"),
				PositionAffinity.Predecessor);
			if (!caretPoint.HasValue)
			{
				return;
			}

			var trackingPoint = caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive);
			session = listener.CompletionBroker.TriggerCompletion(textView, trackingPoint, true);
			return;
		}
	}
}
