using System.Linq;
using Eto.Forms;
using System.Collections.Generic;

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
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, OpenFileDialog.IHandler
	{

		protected override NSOpenPanel CreateControl()
		{
			return NSOpenPanel.OpenPanel;
		}

		protected override bool DisposeControl { get { return false; } }

		public bool MultiSelect
		{
			get { return Control.AllowsMultipleSelection; }
			set { Control.AllowsMultipleSelection = value; }
		}

		public IEnumerable<string> Filenames
		{
			get { return Control.Urls.Select(a => a.Path); }
		}
	}
}
