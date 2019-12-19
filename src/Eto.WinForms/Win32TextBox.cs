using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace Eto.WinForms
{
	public static class Win32TextBox
	{
#pragma warning disable CS0618 // ComInterfaceType.InterfaceIsDual is obsolete: 'Support for IDispatch may be unavailable in future releases.'
		[ComVisible(true), Guid("8CC497C0-A1DF-11ce-8098-00AA0047BE5D"), InterfaceType(ComInterfaceType.InterfaceIsDual), SuppressUnmanagedCodeSecurity]
#pragma warning restore CS0618 // ComInterfaceType.InterfaceIsDual is obsolete: 'Support for IDispatch may be unavailable in future releases.'
		public interface ITextDocument
		{
			string GetName();
			object GetSelection();
			int GetStoryCount();
			object GetStoryRanges();
			int GetSaved();
			void SetSaved(int value);
			object GetDefaultTabStop();
			void SetDefaultTabStop(object value);
			void New();
			void Open(object pVar, int flags, int codePage);
			void Save(object pVar, int flags, int codePage);
			int Freeze();
			int Unfreeze();
			void BeginEditCollection();
			void EndEditCollection();
			int Undo(int count);
			int Redo(int count);
			[return: MarshalAs(UnmanagedType.Interface)]
			ITextRange Range(int cp1, int cp2);
			[return: MarshalAs(UnmanagedType.Interface)]
			ITextRange RangeFromPoint(int x, int y);
		}

#pragma warning disable CS0618 // ComInterfaceType.InterfaceIsDual is obsolete: 'Support for IDispatch may be unavailable in future releases.'
		[ComVisible(true), Guid("8CC497C2-A1DF-11ce-8098-00AA0047BE5D"), InterfaceType(ComInterfaceType.InterfaceIsDual), SuppressUnmanagedCodeSecurity]
#pragma warning restore CS0618 // ComInterfaceType.InterfaceIsDual is obsolete: 'Support for IDispatch may be unavailable in future releases.'
		public interface ITextRange
		{
			string GetText();
			void SetText(string text);
			object GetChar();
			void SetChar(object ch);
			[return: MarshalAs(UnmanagedType.Interface)]
			ITextRange GetDuplicate();
			[return: MarshalAs(UnmanagedType.Interface)]
			ITextRange GetFormattedText();
			void SetFormattedText([MarshalAs(UnmanagedType.Interface)] [In] ITextRange range);
			int GetStart();
			void SetStart(int cpFirst);
			int GetEnd();
			void SetEnd(int cpLim);
			object GetFont();
			void SetFont(object font);
			object GetPara();
			void SetPara(object para);
			int GetStoryLength();
			int GetStoryType();
			void Collapse(int start);
			int Expand(int unit);
			int GetIndex(int unit);
			void SetIndex(int unit, int index, int extend);
			void SetRange(int cpActive, int cpOther);
			int InRange([MarshalAs(UnmanagedType.Interface)] [In] ITextRange range);
			int InStory([MarshalAs(UnmanagedType.Interface)] [In] ITextRange range);
			int IsEqual([MarshalAs(UnmanagedType.Interface)] [In] ITextRange range);
			void Select();
			int StartOf(int unit, int extend);
			int EndOf(int unit, int extend);
			int Move(int unit, int count);
			int MoveStart(int unit, int count);
			int MoveEnd(int unit, int count);
			int MoveWhile(object cset, int count);
			int MoveStartWhile(object cset, int count);
			int MoveEndWhile(object cset, int count);
			int MoveUntil(object cset, int count);
			int MoveStartUntil(object cset, int count);
			int MoveEndUntil(object cset, int count);
			int FindText(string text, int cch, int flags);
			int FindTextStart(string text, int cch, int flags);
			int FindTextEnd(string text, int cch, int flags);
			int Delete(int unit, int count);
			void Cut(out object pVar);
			void Copy(out object pVar);
			void Paste(object pVar, int format);
			int CanPaste(object pVar, int format);
			int CanEdit();
			void ChangeCase(int type);
			void GetPoint(int type, out int x, out int y);
			void SetPoint(int x, int y, int type, int extend);
			void ScrollIntoView(int value);
			object GetEmbeddedObject();
		}

		public static void FastScrollToCaret(this TextBoxBase control)
		{
			if (control.IsHandleCreated)
			{
				var textLength = control.TextLength;
				if (textLength == 0)
				{
					return;
				}
				bool flag = false;
				object iunknown = null;
				var iunkHandle = IntPtr.Zero;
				try
				{
					if (SendMessage(new HandleRef(control, control.Handle), 1084, 0, out iunknown) != 0)
					{
						iunkHandle = Marshal.GetIUnknownForObject(iunknown);
						if (iunkHandle != IntPtr.Zero)
						{
							var itextDocumentHandle = IntPtr.Zero;
							var gUID = typeof(ITextDocument).GUID;
							try
							{
								Marshal.QueryInterface(iunkHandle, ref gUID, out itextDocumentHandle);
								var textDocument = Marshal.GetObjectForIUnknown(itextDocumentHandle) as ITextDocument;
								if (textDocument != null)
								{
									int start = control.SelectionStart;
									int lineFromCharIndex = control.GetLineFromCharIndex(start);
									var textRange = textDocument.Range(start, start + control.SelectionLength);
									textRange.ScrollIntoView(0); // scroll to start of selection
									int num3 = (int)Win32.SendMessage(control.Handle, (Win32.WM)206, IntPtr.Zero, IntPtr.Zero);
									if (num3 > lineFromCharIndex)
									{
										textRange.ScrollIntoView(32);
									}
									flag = true;
								}
							}
							finally
							{
								if (itextDocumentHandle != IntPtr.Zero)
								{
									Marshal.Release(itextDocumentHandle);
								}
							}
						}
					}
				}
				finally
				{
					if (iunkHandle != IntPtr.Zero)
					{
						Marshal.Release(iunkHandle);
					}
				}
				if (!flag)
				{
					Win32.SendMessage(control.Handle, (Win32.WM)183, IntPtr.Zero, IntPtr.Zero);
					return;
				}
			}
			else
				control.ScrollToCaret();
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(HandleRef hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.IUnknown)] out object editOle);
	}
}
