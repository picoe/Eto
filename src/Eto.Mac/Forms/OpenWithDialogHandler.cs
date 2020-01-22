using System;
using System.Diagnostics;
using Eto.Forms;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;

#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<NSOpenPanel, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
	{
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			Control = new NSOpenPanel();
			Control.ReleasedWhenClosed = true;
			Control.DirectoryUrl = new NSUrl("/Applications");
			Control.Prompt = "Choose Application";
			Control.AllowedFileTypes = new[] { "app" };

			if (Control.RunModal() == 1)
				Process.Start("open", "-a \"" + Control.Url.Path +  "\" \"" + FilePath + "\"");

			return DialogResult.Ok;
		}
	}
}
