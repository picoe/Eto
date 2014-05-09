using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public abstract class MacButton<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, ITextControl
		where TControl: NSButton
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public virtual string Text
		{
			get
			{
				return Control.Title;
			}
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				Control.SetTitleWithMnemonic(value);
				LayoutIfNeeded(oldSize);
			}
		}
	}
}

