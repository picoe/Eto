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

namespace Eto.Mac.Forms.Controls
{
	public class LabelHandler : MacLabel<NSTextField, Label, Label.ICallback>, Label.IHandler
	{
	}
}
