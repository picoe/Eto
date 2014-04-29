using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Mac.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<NSOpenPanel, SelectFolderDialog>, ISelectFolderDialog
	{
		public SelectFolderDialogHandler ()
		{
			Control = new NSOpenPanel();
			Control.CanChooseDirectories = true;
			Control.CanChooseFiles = false;
			Control.CanCreateDirectories = true;
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			var ret = Control.RunModal();
			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}
		
		public string Title {
			get {
				return Control.Title;
			}
			set {
				Control.Title = value;
			}
		}
		
		public string Directory {
			get {
				return Control.DirectoryUrl.Path;
			}
			set {
				Control.DirectoryUrl = new NSUrl(value);
			}
		}
		
	}
}

