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

		protected override aw.ProgressBar CreateControl()
		{
			return new aw.ProgressBar(Platform.AppContextThemed)
			{
				Indeterminate = true,
				Visibility = enabled ? av.ViewStates.Visible : av.ViewStates.Invisible
			};
		}

		public override bool Enabled
		{
			get { return enabled; }
			set
				{
				if (enabled == value)
					return;

				enabled = value;

				if (HasControl)
					Control.Visibility = enabled ? av.ViewStates.Visible : av.ViewStates.Invisible;
			}
		}
	}
}

