using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	static class MacControl
	{
		internal static readonly object Font_Key = new object();
	}

	public abstract class MacControl<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl : NSControl
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		public override NSView ContainerControl { get { return Control; } }

		protected override bool ControlEnabled
		{
			get => Control.Enabled;
			set => Control.Enabled = value;
		}

		protected bool HasFont => Widget.Properties.ContainsKey(MacControl.Font_Key);

		public virtual Font Font
		{
			get => Widget.Properties.Get<Font>(MacControl.Font_Key) ?? Widget.Properties.Create(MacControl.Font_Key, () => new Font(new FontHandler(Control.Font)));
			set
			{
				if (Widget.Properties.TrySet(MacControl.Font_Key, value))
				{
					Control.Font = value.ToNS() ?? NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					Control.AttributedStringValue = value.AttributedString(Control.AttributedStringValue);
					InvalidateMeasure();
				};
			}
		}

		protected override IColorizeCell ColorizeCell => Control.Cell as IColorizeCell;

	}
}

