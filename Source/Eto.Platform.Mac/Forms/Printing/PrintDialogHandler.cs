using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<NSPrintPanel, PrintDialog>, IPrintDialog
	{
		public PrintDialogHandler ()
		{
			Control = NSPrintPanel.PrintPanel;
		}

		class SheetHelper : NSObject
		{
			[Export("endSheet:")]
			public void EndSheet (int result)
			{
				NSApplication.SharedApplication.StopModalWithCode (result); 
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			int ret;
			var printInfo = Control.PrintInfo ?? new NSPrintInfo();
			if (parent != null) {
				var parentHandler = parent.Handler as IMacWindow;

				var closeSheet = new SheetHelper();
				Control.BeginSheet (printInfo, parentHandler.Control, closeSheet, new Selector("endSheet:"), IntPtr.Zero);
				ret = NSApplication.SharedApplication.RunModalForWindow (parentHandler.Control);
			}
			else
				ret = Control.RunModalWithPrintInfo (printInfo);


			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
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

