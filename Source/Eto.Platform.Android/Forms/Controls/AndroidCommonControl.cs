using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms.Controls
{
	public abstract class AndroidCommonControl<T, TWidget> : AndroidControl<T, TWidget>, ICommonControl
		where TWidget: CommonControl
		where T: av.View
	{
		public AndroidCommonControl()
		{
		}

		public override av.View ContainerControl
		{
			get { return Control; }
		}

		public Font Font
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}

