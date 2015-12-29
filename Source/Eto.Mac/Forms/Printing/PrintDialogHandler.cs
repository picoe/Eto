using System;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<NSPrintPanel, PrintDialog>, PrintDialog.IHandler
	{
		PrintSettings settings;

		protected override NSPrintPanel CreateControl()
		{
			return new NSPrintPanel();
		}

		class SheetHelper : NSObject
		{
			[Export("printPanelDidEnd:returnCode:contextInfo:")]
			public void PrintPanelDidEnd(NSPrintPanel printPanel, int returnCode, IntPtr contextInfo)
			{
				NSApplication.SharedApplication.StopModalWithCode(returnCode); 
			}
		}

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			int ret;
			var docHandler = Document != null ? Document.Handler as PrintDocumentHandler : null;

			if (docHandler != null)
			{
				Control.Options |= NSPrintPanelOptions.ShowsPreview;
				ret = docHandler.Print(true, parent, Control) ? 1 : 0;
			}
			else
			{
				var printInfo = settings.ToNS();
				if (parent != null)
				{
					var parentHandler = (IMacWindow)parent.Handler;
					var closeSheet = new SheetHelper();
					Control.BeginSheet(printInfo, parentHandler.Control, closeSheet, new Selector("printPanelDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
					ret = (int)NSApplication.SharedApplication.RunModalForWindow(parentHandler.Control);
				}
				else
					ret = (int)Control.RunModalWithPrintInfo(printInfo);
			}

			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get
			{
				if (settings == null)
					settings = Control.PrintInfo.ToEto();
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		public bool AllowCopies
		{
			get { return Control.Options.HasFlag(NSPrintPanelOptions.ShowsCopies); }
			set
			{
				if (value)
					Control.Options |= NSPrintPanelOptions.ShowsCopies;
				else
					Control.Options &= ~NSPrintPanelOptions.ShowsCopies;
			}
		}

		public bool AllowPageRange
		{
			get { return Control.Options.HasFlag(NSPrintPanelOptions.ShowsPageRange); }
			set
			{
				if (value)
					Control.Options |= NSPrintPanelOptions.ShowsPageRange;
				else
					Control.Options &= ~NSPrintPanelOptions.ShowsPageRange;
			}
		}

		public bool AllowSelection
		{
			get { return Control.Options.HasFlag(NSPrintPanelOptions.ShowsPrintSelection); }
			set
			{
				if (value)
					Control.Options |= NSPrintPanelOptions.ShowsPrintSelection;
				else
					Control.Options &= ~NSPrintPanelOptions.ShowsPrintSelection;
			}
		}
	}
}

