using System;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
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
				if (!string.IsNullOrEmpty (ext))
					return ext;
				else
					return base.RequiredFileType;
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

	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, ISaveFileDialog
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
