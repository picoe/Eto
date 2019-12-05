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

namespace Eto.Mac.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<NSOpenPanel, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		protected override NSOpenPanel CreateControl()
		{
			return new NSOpenPanel();
		}

		protected override void Initialize()
		{
			Control.CanChooseDirectories = true;
			Control.CanChooseFiles = false;
			Control.CanCreateDirectories = true;

			base.Initialize();
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			var ret = Control.RunModal();
			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}
		
		public string Title {
			get {
				return Control.Message;
			}
			set {
				Control.Message = value ?? string.Empty;
			}
		}
		
		public string Directory {
			get {
				return Control.Url?.Path ?? Control.DirectoryUrl.Path;
			}
			set {
				Control.DirectoryUrl = NSUrl.FromFilename(value);
			}
		}
		
	}
}

