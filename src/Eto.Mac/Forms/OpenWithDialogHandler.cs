using System;
using System.Diagnostics;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
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
