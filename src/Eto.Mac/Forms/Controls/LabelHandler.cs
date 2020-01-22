using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Text.RegularExpressions;
using System.Linq;

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

#if XAMMAC2
using nnint = System.nint;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.Int32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class LabelHandler : MacLabel<NSTextField, Label, Label.ICallback>, Label.IHandler
	{
	}
}
