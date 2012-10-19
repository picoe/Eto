using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	public static class MacViewExtensions
	{
		public static UIView GetContainerView (this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IiosContainer;
			if (containerHandler != null)
				return containerHandler.ContentControl;
			return control.ControlObject as UIView;
		}
	}
}

