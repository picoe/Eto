using System;
using Eto.Forms;
using Eto.Drawing;
using UIKit;
using Eto.iOS.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public abstract class IosButton<TControl, TWidget, TCallback> : IosControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl: UIButton
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		Font font;

		public virtual string Text
		{
			get
			{
				return Control.Title(UIControlState.Normal);
			}
			set
			{
				LayoutIfNeeded(() => Control.SetTitle(value, UIControlState.Normal));
			}
		}

		public override Eto.Drawing.Font Font
		{
			get { return font; }
			set
			{
				font = value;
				Control.Font = font.ToUI();
			}
		}

		public virtual Color TextColor
		{
			get { return Control.CurrentTitleColor.ToEto(); }
			set { Control.SetTitleColor(value.ToNSUI(), UIControlState.Normal); }
		}
	}
}

