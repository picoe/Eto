using System;
using Eto.Forms;
using UIKit;
using Eto.iOS.Drawing;
using Eto.Drawing;
using Foundation;

namespace Eto.iOS.Forms.Controls
{
	public class IosControl<TControl, TWidget, TCallback> : IosView<TControl, TWidget, TCallback>, Control.IHandler
		where TControl: UIControl
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				Control.Enabled = value;
			}
		}
	}
}

