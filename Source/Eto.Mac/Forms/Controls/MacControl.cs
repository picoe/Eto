using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using Eto.Mac.Drawing;
using sd = System.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public abstract class MacControl<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl: NSControl
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		Font font;

		public override NSView ContainerControl { get { return Control; } }

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public virtual Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.Font));
				return font;
			}
			set
			{
				font = value;
				Control.Font = font.ToNSFont();
				Control.AttributedStringValue = font.AttributedString(Control.AttributedStringValue);
				LayoutIfNeeded();
			}
		}
	}
}

