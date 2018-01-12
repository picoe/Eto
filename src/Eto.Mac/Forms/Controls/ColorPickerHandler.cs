using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ColorPickerHandler : MacControl<NSColorWell, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		public class EtoColorWell : NSColorWell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			ColorPickerHandler Handler => GetHandler(this) as ColorPickerHandler;

			public override void Activate(bool exclusive)
			{
				base.Activate(exclusive);
				var handler = Handler;
				if (handler != null)
				{
					NSColorPanel.SharedColorPanel.ShowsAlpha = handler.AllowAlpha;
					handler.TriggerMouseCallback();
				}
			}

			public override void Deactivate()
			{
				base.Deactivate();
				var handler = Handler;
				if (handler != null)
				{
					handler.TriggerMouseCallback();
				}
			}

			public override NSColor Color
			{
				get
				{
					return base.Color;
				}
				set
				{
					base.Color = value;
					var handler = Handler;
					handler?.Callback.OnColorChanged(handler.Widget, EventArgs.Empty);
				}
			}
		}

		protected override NSColorWell CreateControl()
		{
			return new EtoColorWell();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return new SizeF(44, 23);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					// handled in EtoColorWell
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public Color Color
		{
			get { return Control.Color.ToEto(false); }
			set { Control.Color = value.ToNSUI(); }
		}

		bool allowAlpha;
		public bool AllowAlpha
		{
			get { return allowAlpha; }
			set
			{
				if (allowAlpha != value)
				{
					allowAlpha = value;
					if (Control.IsActive)
					{
						// if it's currently active, set the color panel properties directly
						NSColorPanel.SharedColorPanel.ShowsAlpha = value;
					}
				}
			}
		}

		public bool SupportsAllowAlpha => true;
	}
}
