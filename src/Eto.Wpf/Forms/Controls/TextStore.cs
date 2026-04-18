using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	internal sealed class TextStore :
			Win32.ITextStoreACP,
			Win32.ITfContextOwnerCompositionSink,
			Win32.ITfMouseTrackerACP,
			Win32.ITfTextEditSink,
			Win32.ITfThreadFocusSink,
			Win32.ITfTransitoryExtensionSink,
			IDisposable
	{
		readonly FrameworkElement control;
		readonly Action<string> commitText;
		readonly Action<string, bool> updateComposition;
		readonly Func<RectangleF?> getInputMethodRectangle;

		Win32.ITfThreadMgr threadMgr;
		Win32.ITfDocumentMgr documentMgr;
		Win32.ITfContext context;
		Win32.ITfSource threadMgrSource;
		Win32.ITfSource contextSource;
		Win32.ITextStoreACPSink acpSink;
		IntPtr acpSinkPointer;
		int sinkMask;
		int clientId;
		int editCookie;
		int currentLockType;
		int threadFocusCookie = Win32.TF_INVALID_COOKIE;
		int textEditCookie = Win32.TF_INVALID_COOKIE;
		int transitoryCookie = Win32.TF_INVALID_COOKIE;
		int nextMouseCookie = 1;
		bool activated;
		bool attached;
		bool disposed;
		bool focused;
		int selectionStart;
		int selectionEnd;
		int compositionDepth;
		int compositionStart;
		int compositionEnd;
		string text = string.Empty;
		string compositionText = string.Empty;
		bool? lastCompositionIsActive;
		string pendingCommit;

		public TextStore(FrameworkElement control, Action<string> commitText, Action<string, bool> updateComposition, Func<RectangleF?> getInputMethodRectangle)
		{
			this.control = control ?? throw new ArgumentNullException(nameof(control));
			this.commitText = commitText;
			this.updateComposition = updateComposition;
			this.getInputMethodRectangle = getInputMethodRectangle;
		}

		public bool IsAttached => attached;

		IntPtr Hwnd => (PresentationSource.FromVisual(control) as HwndSource)?.Handle ?? IntPtr.Zero;

		public void Attach()
		{
			if (disposed || attached || !control.IsLoaded)
				return;

			int hr = Win32.TF_GetThreadMgr(out threadMgr);
			if (hr != Win32.S_OK || threadMgr == null)
			{
				hr = Win32.TF_CreateThreadMgr(out threadMgr);
				if (hr != Win32.S_OK || threadMgr == null)
					return;
			}

			hr = threadMgr.Activate(out clientId);
			if (hr != Win32.S_OK)
				return;

			activated = true;

			hr = threadMgr.CreateDocumentMgr(out documentMgr);
			if (hr != Win32.S_OK || documentMgr == null)
				return;

			hr = documentMgr.CreateContext(clientId, 0, this, out context, out editCookie);
			if (hr != Win32.S_OK || context == null)
				return;

			hr = documentMgr.Push(context);
			if (hr != Win32.S_OK)
				return;

			threadMgrSource = threadMgr as Win32.ITfSource;
			if (threadMgrSource != null)
			{
				var iid = Win32.IID_ITfThreadFocusSink;
				threadMgrSource.AdviseSink(ref iid, this, out threadFocusCookie);
			}

			contextSource = context as Win32.ITfSource;
			if (contextSource != null)
			{
				var textEditIid = Win32.IID_ITfTextEditSink;
				contextSource.AdviseSink(ref textEditIid, this, out textEditCookie);

				var transitoryIid = Win32.IID_ITfTransitoryExtensionSink;
				contextSource.AdviseSink(ref transitoryIid, this, out transitoryCookie);
			}

			attached = true;
			SetFocused(focused);
		}

		public void SetFocused(bool value)
		{
			focused = value;

			if (!attached || disposed || threadMgr == null || documentMgr == null)
				return;

			var hwnd = Hwnd;
			if (hwnd == IntPtr.Zero)
				return;

			if (value)
			{
				threadMgr.AssociateFocus(hwnd, documentMgr, out _);
				threadMgr.SetFocus(documentMgr);
				NotifyLayoutChanged();
			}
			else
			{
				ClearComposition();
				threadMgr.AssociateFocus(hwnd, null, out _);
			}
		}

		public void NotifyLayoutChanged()
		{
			if (!attached || acpSink == null || (sinkMask & Win32.TS_AS_LAYOUT_CHANGE) == 0)
				return;

			acpSink.OnLayoutChange(Win32.TsLayoutCode.TS_LC_CHANGE, 0);
		}

		public void CancelComposition()
		{
			if (disposed)
				return;

			pendingCommit = null;
			compositionDepth = 0;
			ClearComposition();

			if (!focused || !attached || threadMgr == null || documentMgr == null)
				return;

			var hwnd = Hwnd;
			if (hwnd == IntPtr.Zero)
				return;

			threadMgr.AssociateFocus(hwnd, null, out _);
			threadMgr.AssociateFocus(hwnd, documentMgr, out _);
			threadMgr.SetFocus(documentMgr);
			NotifyLayoutChanged();
		}

		public void CommitComposition()
		{
			if (disposed)
				return;

			var committed = !string.IsNullOrEmpty(pendingCommit) ? pendingCommit : compositionText;
			pendingCommit = null;
			compositionDepth = 0;
			ClearComposition();

			if (!string.IsNullOrEmpty(committed))
				commitText?.Invoke(committed);

			NotifyLayoutChanged();
		}

		public void Dispose()
		{
			if (disposed)
				return;

			disposed = true;

			try
			{
				SetFocused(false);
			}
			catch
			{
			}

			if (contextSource != null)
			{
				if (textEditCookie != Win32.TF_INVALID_COOKIE)
					contextSource.UnadviseSink(textEditCookie);
				if (transitoryCookie != Win32.TF_INVALID_COOKIE)
					contextSource.UnadviseSink(transitoryCookie);
			}

			if (threadMgrSource != null && threadFocusCookie != Win32.TF_INVALID_COOKIE)
				threadMgrSource.UnadviseSink(threadFocusCookie);

			if (activated && threadMgr != null)
				threadMgr.Deactivate();

			if (acpSinkPointer != IntPtr.Zero)
			{
				Marshal.Release(acpSinkPointer);
				acpSinkPointer = IntPtr.Zero;
			}

			acpSink = null;
			contextSource = null;
			threadMgrSource = null;
			context = null;
			documentMgr = null;
			threadMgr = null;
			attached = false;
		}

		static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;
			return value;
		}

		void SetCompositionText(string value, int start, int end)
		{
			value ??= string.Empty;
			var isActive = compositionDepth > 0;
			var changed = compositionText != value || compositionStart != start || compositionEnd != end || lastCompositionIsActive != isActive;
			compositionText = value;
			compositionStart = start;
			compositionEnd = end;
			if (changed)
			{
				lastCompositionIsActive = isActive;
				updateComposition?.Invoke(compositionText, isActive);
			}
		}

		void UpdateCompositionTextFromRange()
		{
			var start = Clamp(compositionStart, 0, text.Length);
			var end = Clamp(compositionEnd, start, text.Length);
			SetCompositionText(text.Substring(start, end - start), start, end);
		}

		void UpdateCompositionRangeForChange(int start, int end, int newLength)
		{
			var oldLength = end - start;
			var delta = newLength - oldLength;

			if (compositionEnd < compositionStart)
				compositionEnd = compositionStart;

			if (compositionStart == compositionEnd)
			{
				compositionStart = start;
				compositionEnd = start + newLength;
				return;
			}

			if (end < compositionStart)
			{
				compositionStart += delta;
				compositionEnd += delta;
				return;
			}

			if (start > compositionEnd)
				return;

			compositionStart = Math.Min(compositionStart, start);
			compositionEnd = Math.Max(compositionEnd, end) + delta;
			if (compositionEnd < compositionStart)
				compositionEnd = compositionStart;
		}

		bool TryGetRangeText(Win32.ITfRange range, int editCookie, out string value)
		{
			value = null;
			if (range == null)
				return false;

			var buffer = new char[Math.Max(64, text.Length + 32)];
			var hr = range.GetText(editCookie, 0, buffer, buffer.Length, out var count);
			if (hr != Win32.S_OK && hr != Win32.S_FALSE)
				return false;

			value = count > 0 ? new string(buffer, 0, count) : string.Empty;
			return true;
		}

		bool TryGetRangeExtent(Win32.ITfRange range, out int start, out int end)
		{
			start = 0;
			end = 0;

			if (!(range is Win32.ITfRangeACP rangeAcp))
				return false;

			var hr = rangeAcp.GetExtent(out start, out var count);
			if (hr != Win32.S_OK)
				return false;

			end = start + Math.Max(0, count);
			return true;
		}

		void ClearComposition()
		{
			var hadComposition = compositionText.Length > 0 || compositionStart != selectionEnd || compositionEnd != selectionEnd;
			compositionText = string.Empty;
			compositionStart = selectionEnd;
			compositionEnd = selectionEnd;
			pendingCommit = null;
			if (hadComposition || lastCompositionIsActive != false)
			{
				lastCompositionIsActive = false;
				updateComposition?.Invoke(string.Empty, false);
			}
		}

		void ReplaceText(int start, int end, string replacement, out Win32.TS_TEXTCHANGE change)
		{
			start = Clamp(start, 0, text.Length);
			end = Clamp(end, start, text.Length);
			replacement = replacement ?? string.Empty;

			text = text.Remove(start, end - start).Insert(start, replacement);
			selectionStart = selectionEnd = start + replacement.Length;

			change = new Win32.TS_TEXTCHANGE
			{
				acpStart = start,
				acpOldEnd = end,
				acpNewEnd = start + replacement.Length
			};

			if (acpSink != null)
			{
				if ((sinkMask & Win32.TS_AS_TEXT_CHANGE) != 0)
					acpSink.OnTextChange(0, ref change);
				if ((sinkMask & Win32.TS_AS_SEL_CHANGE) != 0)
					acpSink.OnSelectionChange();
			}

			if (replacement.Length == 0)
			{
				if (compositionDepth > 0)
				{
					UpdateCompositionRangeForChange(start, end, 0);
					UpdateCompositionTextFromRange();
				}
				return;
			}

			if (compositionDepth > 0)
			{
				UpdateCompositionRangeForChange(start, end, replacement.Length);
				UpdateCompositionTextFromRange();
				return;
			}

			commitText?.Invoke(replacement);
		}

		bool TryGetControlScreenRect(out Win32.RECT rect)
		{
			rect = default;

			if (!control.IsLoaded || control.ActualWidth <= 0 || control.ActualHeight <= 0)
				return false;

			var inputRect = getInputMethodRectangle?.Invoke();
			var localRect = inputRect ?? new RectangleF(0, 0, 1, Math.Max(1, (float)control.ActualHeight));
			if (localRect.Width <= 0)
				localRect.Width = 1;
			if (localRect.Height <= 0)
				localRect.Height = 1;

			var topLeft = control.PointToScreen(new System.Windows.Point(localRect.X, localRect.Y));
			rect.left = (int)Math.Round(topLeft.X);
			rect.top = (int)Math.Round(topLeft.Y);
			rect.right = rect.left + (int)Math.Ceiling(localRect.Width);
			rect.bottom = rect.top + (int)Math.Ceiling(localRect.Height);
			return true;
		}

		public int AdviseSink(ref Guid riid, IntPtr punk, int mask)
		{
			if (riid != Win32.IID_ITextStoreACPSink)
				return Win32.E_INVALIDARG;
			if (punk == IntPtr.Zero)
				return Win32.E_INVALIDARG;
			if (acpSinkPointer != IntPtr.Zero && acpSinkPointer != punk)
				return Win32.CONNECT_E_ADVISELIMIT;

			if (acpSinkPointer == IntPtr.Zero)
			{
				acpSinkPointer = punk;
				Marshal.AddRef(acpSinkPointer);
				acpSink = (Win32.ITextStoreACPSink)Marshal.GetObjectForIUnknown(acpSinkPointer);
			}

			sinkMask = mask;
			return Win32.S_OK;
		}

		public int UnadviseSink(IntPtr punk)
		{
			if (acpSinkPointer == IntPtr.Zero)
				return Win32.CONNECT_E_NOCONNECTION;
			if (punk != IntPtr.Zero && punk != acpSinkPointer)
				return Win32.CONNECT_E_NOCONNECTION;

			Marshal.Release(acpSinkPointer);
			acpSinkPointer = IntPtr.Zero;
			acpSink = null;
			sinkMask = 0;
			return Win32.S_OK;
		}

		public int RequestLock(int lockFlags, out int sessionResult)
		{
			sessionResult = Win32.S_OK;

			if (acpSink == null)
				return Win32.E_FAIL;

			if (currentLockType != 0)
			{
				sessionResult = Win32.TS_E_SYNCHRONOUS;
				return Win32.TS_S_ASYNC;
			}

			currentLockType = lockFlags;
			try
			{
				return acpSink.OnLockGranted(lockFlags);
			}
			finally
			{
				currentLockType = 0;
			}
		}

		public int GetStatus(out Win32.TS_STATUS status)
		{
			status = new Win32.TS_STATUS
			{
				dwDynamicFlags = Win32.TS_SD_UIINTEGRATIONENABLE,
				dwStaticFlags = Win32.TS_SS_TRANSITORY | Win32.TS_SS_NOHIDDENTEXT
			};
			return Win32.S_OK;
		}

		public int QueryInsert(int acpTestStart, int acpTestEnd, int cch, out int acpResultStart, out int acpResultEnd)
		{
			acpResultStart = Clamp(acpTestStart, 0, text.Length);
			acpResultEnd = acpResultStart + Math.Max(0, cch);
			return Win32.S_OK;
		}

		public int GetSelection(int index, int count, Win32.TS_SELECTION_ACP[] selection, out int fetchedCount)
		{
			fetchedCount = 0;
			if (selection == null || selection.Length == 0 || count <= 0)
				return Win32.E_INVALIDARG;
			if (index != 0 && index != Win32.TF_DEFAULT_SELECTION)
				return Win32.TS_E_NOSELECTION;

			selection[0] = new Win32.TS_SELECTION_ACP
			{
				acpStart = selectionStart,
				acpEnd = selectionEnd,
				style_ase = Win32.TsActiveSelEnd.TS_AE_END,
				style_fInterimChar = false
			};
			fetchedCount = 1;
			return Win32.S_OK;
		}

		public int SetSelection(int count, Win32.TS_SELECTION_ACP[] selection)
		{
			if (selection == null || selection.Length == 0 || count <= 0)
				return Win32.E_INVALIDARG;

			selectionStart = Clamp(selection[0].acpStart, 0, text.Length);
			selectionEnd = Clamp(selection[0].acpEnd, 0, text.Length);

			if (acpSink != null && (sinkMask & Win32.TS_AS_SEL_CHANGE) != 0)
				acpSink.OnSelectionChange();

			if (compositionDepth > 0)
			{
				UpdateCompositionTextFromRange();
			}

			return Win32.S_OK;
		}

		public int GetText(int acpStart, int acpEnd, char[] plainText, int plainTextLength, out int plainTextReturned, Win32.TS_RUNINFO[] runInfo, int runInfoLength, out int runInfoReturned, out int nextAcp)
		{
			plainTextReturned = 0;
			runInfoReturned = 0;

			var start = Clamp(acpStart, 0, text.Length);
			var end = acpEnd < 0 ? text.Length : Clamp(acpEnd, start, text.Length);
			var range = text.Substring(start, end - start);

			if (plainText != null && plainTextLength > 0)
			{
				plainTextReturned = Math.Min(range.Length, plainTextLength);
				range.CopyTo(0, plainText, 0, plainTextReturned);
			}

			if (runInfo != null && runInfo.Length > 0)
			{
				runInfo[0] = new Win32.TS_RUNINFO
				{
					uCount = range.Length,
					type = Win32.TsRunType.TS_RT_PLAIN
				};
				runInfoReturned = 1;
			}

			nextAcp = end;
			return Win32.S_OK;
		}

		public int SetText(int flags, int acpStart, int acpEnd, char[] replacementText, int length, out Win32.TS_TEXTCHANGE change)
		{
			var replacement = replacementText == null || length <= 0 ? string.Empty : new string(replacementText, 0, Math.Min(length, replacementText.Length));
			ReplaceText(acpStart, acpEnd, replacement, out change);
			return Win32.S_OK;
		}

		public int GetFormattedText(int acpStart, int acpEnd, out System.Runtime.InteropServices.ComTypes.IDataObject dataObject)
		{
			dataObject = null;
			return Win32.E_NOTIMPL;
		}

		public int GetEmbedded(int acpPos, ref Guid service, ref Guid riid, out IntPtr unk)
		{
			unk = IntPtr.Zero;
			return Win32.TS_E_NOOBJECT;
		}

		public int QueryInsertEmbedded(IntPtr guidService, IntPtr formatEtc, out bool insertable)
		{
			insertable = false;
			return Win32.S_OK;
		}

		public int InsertEmbedded(int flags, int acpStart, int acpEnd, System.Runtime.InteropServices.ComTypes.IDataObject dataObject, out Win32.TS_TEXTCHANGE change)
		{
			change = default;
			return Win32.E_NOTIMPL;
		}

		public int InsertTextAtSelection(int flags, char[] replacementText, int length, out int acpStart, out int acpEnd, out Win32.TS_TEXTCHANGE change)
		{
			acpStart = selectionStart;
			acpEnd = selectionEnd;
			var replacement = replacementText == null || length <= 0 ? string.Empty : new string(replacementText, 0, Math.Min(length, replacementText.Length));
			ReplaceText(selectionStart, selectionEnd, replacement, out change);
			return Win32.S_OK;
		}

		public int InsertEmbeddedAtSelection(int flags, System.Runtime.InteropServices.ComTypes.IDataObject dataObject, out int acpStart, out int acpEnd, out Win32.TS_TEXTCHANGE change)
		{
			acpStart = selectionStart;
			acpEnd = selectionEnd;
			change = default;
			return Win32.E_NOTIMPL;
		}

		public int RequestSupportedAttrs(int flags, int filterAttrCount, IntPtr filterAttrs) => Win32.S_OK;

		public int RequestAttrsAtPosition(int acpPos, int filterAttrCount, IntPtr filterAttrs, int flags) => Win32.S_OK;

		public int RequestAttrsTransitioningAtPosition(int acpPos, int filterAttrCount, IntPtr filterAttrs, int flags) => Win32.S_OK;

		public int FindNextAttrTransition(int acpStart, int acpHalt, int filterAttrCount, IntPtr filterAttrs, int flags, out int acpNext, out bool found, out int foundOffset)
		{
			acpNext = Clamp(acpHalt, 0, text.Length);
			found = false;
			foundOffset = 0;
			return Win32.S_OK;
		}

		public int RetrieveRequestedAttrs(int count, IntPtr attrValues, out int fetchedCount)
		{
			fetchedCount = 0;
			return Win32.S_OK;
		}

		public int GetEndACP(out int acp)
		{
			acp = text.Length;
			return Win32.S_OK;
		}

		public int GetActiveView(out int viewCookie)
		{
			viewCookie = 0;
			return Win32.S_OK;
		}

		public int GetACPFromPoint(int viewCookie, ref Win32.POINT point, int flags, out int acp)
		{
			acp = compositionDepth > 0 ? compositionEnd : selectionEnd;
			return Win32.S_OK;
		}

		public int GetTextExt(int viewCookie, int acpStart, int acpEnd, out Win32.RECT rect, out bool clipped)
		{
			clipped = false;
			if (!TryGetControlScreenRect(out rect))
				return Win32.TS_E_NOLAYOUT;
			return Win32.S_OK;
		}

		public int GetScreenExt(int viewCookie, out Win32.RECT rect)
		{
			if (!TryGetControlScreenRect(out rect))
				return Win32.TS_E_NOLAYOUT;
			return Win32.S_OK;
		}

		public int GetWnd(int viewCookie, out IntPtr hwnd)
		{
			hwnd = Hwnd;
			return hwnd != IntPtr.Zero ? Win32.S_OK : Win32.TS_E_NOLAYOUT;
		}

		public int OnSetThreadFocus()
		{
			NotifyLayoutChanged();
			return Win32.S_OK;
		}

		public int OnKillThreadFocus() => Win32.S_OK;

		public int OnStartComposition(Win32.ITfCompositionView composition, out bool ok)
		{
			compositionDepth++;
			SetCompositionText(compositionText, selectionStart, selectionEnd);
			ok = true;
			return Win32.S_OK;
		}

		public int OnUpdateComposition(Win32.ITfCompositionView composition, Win32.ITfRange rangeNew)
		{
			NotifyLayoutChanged();
			return Win32.S_OK;
		}

		public int OnEndComposition(Win32.ITfCompositionView composition)
		{
			if (compositionDepth > 0)
				compositionDepth--;

			if (compositionDepth == 0 && string.IsNullOrEmpty(pendingCommit) && !string.IsNullOrEmpty(compositionText))
			{
				pendingCommit = compositionText;
			}

			if (compositionDepth == 0 && !string.IsNullOrEmpty(pendingCommit))
			{
				var committed = pendingCommit;
				ClearComposition();
				commitText?.Invoke(committed);
			}
			else if (compositionDepth == 0)
			{
				ClearComposition();
			}

			return Win32.S_OK;
		}

		public int OnEndEdit(Win32.ITfContext context, int readOnlyEditCookie, Win32.ITfEditRecord editRecord) => Win32.S_OK;

		public int OnTransitoryExtensionUpdated(Win32.ITfContext context, int readOnlyEditCookie, Win32.ITfRange resultRange, Win32.ITfRange compositionRange, out bool deleteResultRange)
		{
			deleteResultRange = false;
			pendingCommit = null;

			if (resultRange != null && TryGetRangeText(resultRange, readOnlyEditCookie, out var resultText))
				pendingCommit = resultText;

			if (compositionRange != null)
			{
				if (TryGetRangeText(compositionRange, readOnlyEditCookie, out var value))
				{
					if (TryGetRangeExtent(compositionRange, out var start, out var end))
						SetCompositionText(value, start, end);
					else
						SetCompositionText(value, compositionStart, compositionStart + value.Length);
				}
			}

			return Win32.S_OK;
		}

		public int AdviseMouseSink(Win32.ITfRangeACP range, Win32.ITfMouseSink sink, out int cookie)
		{
			cookie = nextMouseCookie++;
			return Win32.S_OK;
		}

		public int UnadviseMouseSink(int cookie) => Win32.S_OK;
	}
}
