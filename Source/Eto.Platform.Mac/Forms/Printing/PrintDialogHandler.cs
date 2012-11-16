using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<NSPrintPanel, PrintDialog>, IPrintDialog
	{
		PrintSettings settings;

		public PrintDialogHandler ()
		{
			Control = NSPrintPanel.PrintPanel;
		}

		class SheetHelper : NSObject
		{
			[Export("printPanelDidEnd:returnCode:contextInfo:")]
			public void PrintPanelDidEnd(NSPrintPanel printPanel, int returnCode, IntPtr contextInfo)
			{
				NSApplication.SharedApplication.StopModalWithCode (returnCode); 
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			int ret;
			var printInfo = settings.ToNS ();
			if (parent != null) {
				var parentHandler = parent.Handler as IMacWindow;
				var closeSheet = new SheetHelper();
				Control.BeginSheet (printInfo, parentHandler.Control, closeSheet, new Selector("printPanelDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
				ret = NSApplication.SharedApplication.RunModalForWindow (parentHandler.Control);
			}
			else
				ret = Control.RunModalWithPrintInfo (printInfo);

			Console.WriteLine (printInfo.PrintSettings);

			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get {
				if (settings == null)
					settings = Control.PrintInfo.ToEto (Widget.Generator);
				return settings;
			}
			set {
				settings = value;
			}
		}


		public bool AllowCopies
		{
			get { return Control.Options.HasFlag (NSPrintPanelOptions.ShowsCopies); }
			set
			{
				if (value) Control.Options |= NSPrintPanelOptions.ShowsCopies;
				else Control.Options &= ~NSPrintPanelOptions.ShowsCopies;
			}
		}

		public bool AllowPageRange
		{
			get { return Control.Options.HasFlag (NSPrintPanelOptions.ShowsPageRange); }
			set
			{
				if (value) Control.Options |= NSPrintPanelOptions.ShowsPageRange;
				else Control.Options &= ~NSPrintPanelOptions.ShowsPageRange;
			}
		}

		public bool AllowSelection
		{
			get { return Control.Options.HasFlag (NSPrintPanelOptions.ShowsPrintSelection); }
			set
			{
				if (value) Control.Options |= NSPrintPanelOptions.ShowsPrintSelection;
				else Control.Options &= ~NSPrintPanelOptions.ShowsPrintSelection;
			}
		}
	}
}

