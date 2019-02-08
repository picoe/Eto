using System;
using Eto.Forms;
using System.IO;
using Eto.Mac.Drawing;
using Eto.Drawing;

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
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		protected override NSPasteboard CreateControl() => NSPasteboard.GeneralPasteboard;

		protected override bool DisposeControl => false;

		/*
		public DataObject DataObject
		{
			get
			{
				return new DataObject(new DataObjectHandler(Control.MutableCopy() as NSPasteboard));
			}
			set
			{
				Control.ClearContents();
				var handler = value?.Handler as IDataObjectHandler;
				handler?.Apply(Control);
			}
		}
		*/

	}
}

