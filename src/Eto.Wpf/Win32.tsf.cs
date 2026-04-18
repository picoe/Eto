using System;
using System.Runtime.InteropServices;

namespace Eto
{
	partial class Win32
	{
		internal const int S_OK = 0x00000000;
		internal const int S_FALSE = 0x00000001;
		internal const int E_FAIL = unchecked((int)0x80004005);
		internal const int E_INVALIDARG = unchecked((int)0x80070057);
		internal const int E_NOTIMPL = unchecked((int)0x80004001);
		internal const int CONNECT_E_ADVISELIMIT = unchecked((int)0x80040200);
		internal const int CONNECT_E_NOCONNECTION = unchecked((int)0x80040204);

		internal const int TS_E_INVALIDPOS = unchecked((int)0x80040200);
		internal const int TS_E_NOLOCK = unchecked((int)0x80040201);
		internal const int TS_E_NOOBJECT = unchecked((int)0x80040202);
		internal const int TS_E_NOSELECTION = unchecked((int)0x80040205);
		internal const int TS_E_NOLAYOUT = unchecked((int)0x80040206);
		internal const int TS_E_INVALIDPOINT = unchecked((int)0x80040207);
		internal const int TS_E_SYNCHRONOUS = unchecked((int)0x80040208);
		internal const int TS_E_READONLY = unchecked((int)0x80040209);
		internal const int TS_S_ASYNC = 0x00040300;

		internal const int TS_AS_TEXT_CHANGE = 0x01;
		internal const int TS_AS_SEL_CHANGE = 0x02;
		internal const int TS_AS_LAYOUT_CHANGE = 0x04;
		internal const int TS_AS_ATTR_CHANGE = 0x08;
		internal const int TS_AS_STATUS_CHANGE = 0x10;

		internal const int TS_LF_SYNC = 0x01;
		internal const int TS_LF_READ = 0x02;
		internal const int TS_LF_READWRITE = 0x06;

		internal const int TS_SD_READONLY = 0x001;
		internal const int TS_SD_LOADING = 0x002;
		internal const int TS_SD_UIINTEGRATIONENABLE = 0x020;

		internal const int TS_SS_TRANSITORY = 0x004;
		internal const int TS_SS_NOHIDDENTEXT = 0x008;

		internal const int TF_INVALID_COOKIE = -1;
		internal const int TF_DEFAULT_SELECTION = unchecked((int)0xFFFFFFFF);

		internal static readonly Guid IID_ITextStoreACP = new Guid("28888FE3-C2A0-483A-A3EA-8CB1CE51FF3D");
		internal static readonly Guid IID_ITextStoreACPSink = new Guid("22D44C94-A419-4542-A272-AE26093ECECF");
		internal static readonly Guid IID_ITfThreadFocusSink = new Guid("C0F1DB0C-3A20-405C-A303-96B6010A885F");
		internal static readonly Guid IID_ITfContextOwnerCompositionSink = new Guid("5F20AA40-B57A-4F34-96AB-3576F377CC79");
		internal static readonly Guid IID_ITfTextEditSink = new Guid("8127D409-CCD3-4683-967A-B43D5B482BF7");
		internal static readonly Guid IID_ITfTransitoryExtensionSink = new Guid("A615096F-1C57-4813-8A15-55EE6E5A839C");
		internal static readonly Guid IID_ITfMouseTrackerACP = new Guid("3BDD78E2-C16E-47FD-B883-CE6FACC1A208");

		[DllImport("msctf.dll")]
		internal static extern int TF_CreateThreadMgr(out ITfThreadMgr threadMgr);

		[DllImport("msctf.dll")]
		internal static extern int TF_GetThreadMgr(out ITfThreadMgr threadMgr);

		[StructLayout(LayoutKind.Sequential)]
		internal struct TS_STATUS
		{
			public int dwDynamicFlags;
			public int dwStaticFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct TS_TEXTCHANGE
		{
			public int acpStart;
			public int acpOldEnd;
			public int acpNewEnd;
		}

		internal enum TsActiveSelEnd
		{
			TS_AE_NONE = 0,
			TS_AE_START = 1,
			TS_AE_END = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct TS_SELECTION_ACP
		{
			public int acpStart;
			public int acpEnd;
			public TsActiveSelEnd style_ase;

			[MarshalAs(UnmanagedType.Bool)]
			public bool style_fInterimChar;
		}

		internal enum TsRunType
		{
			TS_RT_PLAIN = 0,
			TS_RT_HIDDEN = 1,
			TS_RT_OPAQUE = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct TS_RUNINFO
		{
			public int uCount;
			public TsRunType type;
		}

		internal enum TsLayoutCode
		{
			TS_LC_CREATE = 0,
			TS_LC_CHANGE = 1,
			TS_LC_DESTROY = 2
		}

		[ComImport]
		[Guid("AA80E7FD-2021-11D2-93E0-0060B067B86E")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfContext
		{
		}

		[ComImport]
		[Guid("2433BF8E-0F9B-435C-BA2C-180611978C30")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfContextView
		{
		}

		[ComImport]
		[Guid("4EA48A35-60AE-446F-8FD6-E6A8D82459F7")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfSource
		{
			[PreserveSig]
			int AdviseSink(ref Guid riid, [MarshalAs(UnmanagedType.Interface)] object punk, out int cookie);

			[PreserveSig]
			int UnadviseSink(int cookie);
		}

		[ComImport]
		[Guid("AA80E7F4-2021-11D2-93E0-0060B067B86E")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfDocumentMgr
		{
			[PreserveSig]
			int CreateContext(int clientId, int flags, [MarshalAs(UnmanagedType.Interface)] object textStore, out ITfContext context, out int editCookie);

			[PreserveSig]
			int Push([MarshalAs(UnmanagedType.Interface)] ITfContext context);
		}

		[ComImport]
		[Guid("AA80E801-2021-11D2-93E0-0060B067B86E")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfThreadMgr
		{
			[PreserveSig]
			int Activate(out int clientId);

			[PreserveSig]
			int Deactivate();

			[PreserveSig]
			int CreateDocumentMgr(out ITfDocumentMgr documentMgr);

			[PreserveSig]
			int EnumDocumentMgrs(out IntPtr enumDocumentMgrs);

			[PreserveSig]
			int GetFocus(out ITfDocumentMgr documentMgr);

			[PreserveSig]
			int SetFocus([MarshalAs(UnmanagedType.Interface)] ITfDocumentMgr documentMgr);

			[PreserveSig]
			int AssociateFocus(IntPtr hwnd, [MarshalAs(UnmanagedType.Interface)] ITfDocumentMgr documentMgr, out ITfDocumentMgr previousDocumentMgr);

			[PreserveSig]
			int IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool threadFocus);
		}

		[ComImport]
		[Guid("22D44C94-A419-4542-A272-AE26093ECECF")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITextStoreACPSink
		{
			[PreserveSig]
			int OnTextChange(int flags, ref TS_TEXTCHANGE change);

			[PreserveSig]
			int OnSelectionChange();

			[PreserveSig]
			int OnLayoutChange(TsLayoutCode layoutCode, int viewCookie);

			[PreserveSig]
			int OnStatusChange(int flags);

			[PreserveSig]
			int OnAttrsChange(int acpStart, int acpEnd, int attrCount, IntPtr attrs);

			[PreserveSig]
			int OnLockGranted(int lockFlags);

			[PreserveSig]
			int OnStartEditTransaction();

			[PreserveSig]
			int OnEndEditTransaction();
		}

		[ComImport]
		[Guid("28888FE3-C2A0-483A-A3EA-8CB1CE51FF3D")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITextStoreACP
		{
			[PreserveSig]
			int AdviseSink(ref Guid riid, IntPtr punk, int mask);

			[PreserveSig]
			int UnadviseSink(IntPtr punk);

			[PreserveSig]
			int RequestLock(int lockFlags, out int sessionResult);

			[PreserveSig]
			int GetStatus(out TS_STATUS status);

			[PreserveSig]
			int QueryInsert(int acpTestStart, int acpTestEnd, int cch, out int acpResultStart, out int acpResultEnd);

			[PreserveSig]
			int GetSelection(int index, int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] TS_SELECTION_ACP[] selection, out int fetchedCount);

			[PreserveSig]
			int SetSelection(int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] TS_SELECTION_ACP[] selection);

			[PreserveSig]
			int GetText(int acpStart, int acpEnd, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] char[] text, int textLength, out int textLengthReturned, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] TS_RUNINFO[] runInfo, int runInfoLength, out int runInfoReturned, out int nextAcp);

			[PreserveSig]
			int SetText(int flags, int acpStart, int acpEnd, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] char[] text, int length, out TS_TEXTCHANGE change);

			[PreserveSig]
			int GetFormattedText(int acpStart, int acpEnd, out System.Runtime.InteropServices.ComTypes.IDataObject dataObject);

			[PreserveSig]
			int GetEmbedded(int acpPos, ref Guid service, ref Guid riid, out IntPtr unk);

			[PreserveSig]
			int QueryInsertEmbedded(IntPtr guidService, IntPtr formatEtc, [MarshalAs(UnmanagedType.Bool)] out bool insertable);

			[PreserveSig]
			int InsertEmbedded(int flags, int acpStart, int acpEnd, System.Runtime.InteropServices.ComTypes.IDataObject dataObject, out TS_TEXTCHANGE change);

			[PreserveSig]
			int InsertTextAtSelection(int flags, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] text, int length, out int acpStart, out int acpEnd, out TS_TEXTCHANGE change);

			[PreserveSig]
			int InsertEmbeddedAtSelection(int flags, System.Runtime.InteropServices.ComTypes.IDataObject dataObject, out int acpStart, out int acpEnd, out TS_TEXTCHANGE change);

			[PreserveSig]
			int RequestSupportedAttrs(int flags, int filterAttrCount, IntPtr filterAttrs);

			[PreserveSig]
			int RequestAttrsAtPosition(int acpPos, int filterAttrCount, IntPtr filterAttrs, int flags);

			[PreserveSig]
			int RequestAttrsTransitioningAtPosition(int acpPos, int filterAttrCount, IntPtr filterAttrs, int flags);

			[PreserveSig]
			int FindNextAttrTransition(int acpStart, int acpHalt, int filterAttrCount, IntPtr filterAttrs, int flags, out int acpNext, [MarshalAs(UnmanagedType.Bool)] out bool found, out int foundOffset);

			[PreserveSig]
			int RetrieveRequestedAttrs(int count, IntPtr attrValues, out int fetchedCount);

			[PreserveSig]
			int GetEndACP(out int acp);

			[PreserveSig]
			int GetActiveView(out int viewCookie);

			[PreserveSig]
			int GetACPFromPoint(int viewCookie, ref POINT point, int flags, out int acp);

			[PreserveSig]
			int GetTextExt(int viewCookie, int acpStart, int acpEnd, out RECT rect, [MarshalAs(UnmanagedType.Bool)] out bool clipped);

			[PreserveSig]
			int GetScreenExt(int viewCookie, out RECT rect);

			[PreserveSig]
			int GetWnd(int viewCookie, out IntPtr hwnd);
		}

		[ComImport]
		[Guid("C0F1DB0C-3A20-405C-A303-96B6010A885F")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfThreadFocusSink
		{
			[PreserveSig]
			int OnSetThreadFocus();

			[PreserveSig]
			int OnKillThreadFocus();
		}

		[ComImport]
		[Guid("5F20AA40-B57A-4F34-96AB-3576F377CC79")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfContextOwnerCompositionSink
		{
			[PreserveSig]
			int OnStartComposition([MarshalAs(UnmanagedType.Interface)] ITfCompositionView composition, [MarshalAs(UnmanagedType.Bool)] out bool ok);

			[PreserveSig]
			int OnUpdateComposition([MarshalAs(UnmanagedType.Interface)] ITfCompositionView composition, [MarshalAs(UnmanagedType.Interface)] ITfRange rangeNew);

			[PreserveSig]
			int OnEndComposition([MarshalAs(UnmanagedType.Interface)] ITfCompositionView composition);
		}

		[ComImport]
		[Guid("8127D409-CCD3-4683-967A-B43D5B482BF7")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfTextEditSink
		{
			[PreserveSig]
			int OnEndEdit([MarshalAs(UnmanagedType.Interface)] ITfContext context, int readOnlyEditCookie, [MarshalAs(UnmanagedType.Interface)] ITfEditRecord editRecord);
		}

		[ComImport]
		[Guid("A615096F-1C57-4813-8A15-55EE6E5A839C")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfTransitoryExtensionSink
		{
			[PreserveSig]
			int OnTransitoryExtensionUpdated([MarshalAs(UnmanagedType.Interface)] ITfContext context, int readOnlyEditCookie, [MarshalAs(UnmanagedType.Interface)] ITfRange resultRange, [MarshalAs(UnmanagedType.Interface)] ITfRange compositionRange, [MarshalAs(UnmanagedType.Bool)] out bool deleteResultRange);
		}

		[ComImport]
		[Guid("3BDD78E2-C16E-47FD-B883-CE6FACC1A208")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfMouseTrackerACP
		{
			[PreserveSig]
			int AdviseMouseSink([MarshalAs(UnmanagedType.Interface)] ITfRangeACP range, [MarshalAs(UnmanagedType.Interface)] ITfMouseSink sink, out int cookie);

			[PreserveSig]
			int UnadviseMouseSink(int cookie);
		}

		[ComImport]
		[Guid("A1ADAAA2-3A24-449D-AC96-5183E7F5C217")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfMouseSink
		{
		}

		[ComImport]
		[Guid("42D4D099-7C1A-4A89-B836-6C6F22160DF0")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfEditRecord
		{
		}

		[ComImport]
		[Guid("D7540241-F9A1-4364-BEFC-DBCD2C4395B7")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfCompositionView
		{
		}

		[ComImport]
		[Guid("AA80E7FF-2021-11D2-93E0-0060B067B86E")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfRange
		{
			[PreserveSig]
			int GetText(int editCookie, int flags, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] char[] text, int textLength, out int textLengthReturned);
		}

		[ComImport]
		[Guid("057A6296-029B-4154-B79A-0D461D4EA94C")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfRangeACP
			: ITfRange
		{
			[PreserveSig]
			int GetExtent(out int anchor, out int count);
		}
	}
}
