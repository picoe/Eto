using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.Foundation;
using System.Collections.Generic;
namespace Eto.Platform.Mac
{
	public abstract class MacControl<T, W> : MacView<T, W>
		where T: NSControl
		where W: Control
	{
		
		public override bool Enabled {
			get {
				return Control.Enabled;
			}
			set {
				Control.Enabled = value;
			}
		}
		
	}
}

