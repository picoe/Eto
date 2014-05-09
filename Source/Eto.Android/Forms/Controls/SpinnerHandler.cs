using System;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class SpinnerHandler : AndroidControl<aw.ProgressBar, Spinner, Spinner.ICallback>, Spinner.IHandler
	{
		bool enabled;

		public override av.View ContainerControl { get { return Control; } }

		public SpinnerHandler()
		{
			Control = new aw.ProgressBar(aa.Application.Context);
			Control.Indeterminate = true;
		}

		public override bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (enabled)
						Control.Visibility = av.ViewStates.Visible;
					else
						Control.Visibility = av.ViewStates.Invisible;
				}
			}
		}
	}
}

