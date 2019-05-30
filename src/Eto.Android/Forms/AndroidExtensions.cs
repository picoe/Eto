using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms
{
	public static class AndroidExtensions
	{
		public static IAndroidControl GetAndroidControl(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IAndroidControl;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			if (child != null)
				return child.GetAndroidControl();
			return null;
		}

		public static av.View GetContainerView(this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IAndroidControl;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContainerView();
			return control.ControlObject as av.View;
		}
	}
}