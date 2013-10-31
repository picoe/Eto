using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public abstract class MacButton<TControl, TWidget> : MacControl<TControl, TWidget>, ITextControl
		where TControl: NSButton
		where TWidget: Control
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

