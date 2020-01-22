using System;
using Eto.Forms;
using Eto.Drawing;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

namespace Eto.Mac.Forms.Controls
{
	public class PanelHandler : MacPanel<NSView, Panel, Panel.ICallback>, Panel.IHandler
	{
		protected override NSView CreateControl() => new MacPanelView();
		
		public override NSView ContainerControl => Control;
	}
}
