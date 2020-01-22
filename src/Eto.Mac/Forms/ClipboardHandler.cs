using System;
using Eto.Forms;
using System.IO;
using Eto.Mac.Drawing;
using Eto.Drawing;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
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

