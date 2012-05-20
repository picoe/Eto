using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public abstract class MacButton<T, W> : MacControl<T, W>, ITextControl
		where T: NSButton
		where W: Control
	{	

		public virtual string Text {
			get {
				return Control.Title;
			}
			set {
				Control.SetTitleWithMnemonic(value);
				LayoutIfNeeded();
			}
		}

	}
}

