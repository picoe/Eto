using System;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Mac.Forms
{
	[Register("MySavePanel")]
	class MySavePanel : NSSavePanel
	{
		public IMacFileDialog Handler { get; set; }
			
		public MySavePanel ()
		{
		}
			
		public MySavePanel (IntPtr handle)
				: base(handle)
		{
				
		}
	
		[Export ("initWithCoder:")]
			public MySavePanel (NSCoder coder)
				: base(coder)
		{
		}

		[Obsolete]
		public override string RequiredFileType {
			get {
				var ext = Handler.GetDefaultExtension ();
				return !string.IsNullOrEmpty(ext) ? ext : base.RequiredFileType;
			}
			set {
				base.RequiredFileType = value;
			}
		}
		
		/*
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}*/
			
	}

	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		public override string FileName {
			get {
				return base.FileName;
			}
			set {
				Control.NameFieldStringValue = value;
			}
		}
		
		public SaveFileDialogHandler ()
		{
			//MySavePanel.Sa
			Control = NSSavePanel.SavePanel; //new MySavePanel{ Handler = this };
			Control.AllowsOtherFileTypes = true;
			Control.CanSelectHiddenExtension = true;
		}


	}
}
