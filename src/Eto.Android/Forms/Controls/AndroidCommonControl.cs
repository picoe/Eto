using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms.Controls
{
	public abstract class AndroidCommonControl<TControl, TWidget, TCallback> : AndroidControl<TControl, TWidget, TCallback>, CommonControl.IHandler
		where TWidget: CommonControl
		where TControl: av.View
		where TCallback: CommonControl.ICallback
	{
		public override av.View ContainerControl
		{
			get { return Control; }
		}

		// TODO
		public virtual Font Font
		{
			get;
			set;
		}

		public override Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
			set
			{ 
				// TODO: need to change to desired size, not min size.. e.g. if control is in a container
				Control.SetMinimumWidth(value.Width);
				Control.SetMinimumHeight(value.Height);
			}
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
	}
}