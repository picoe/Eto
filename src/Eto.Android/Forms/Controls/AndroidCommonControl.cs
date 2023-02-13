using System;
using Eto.Forms;
using Eto.Drawing;

using av = Android.Views;
using aw = Android.Widget;
using au = Android.Util;

namespace Eto.Android.Forms.Controls
{
	public abstract class AndroidCommonControl<TControl, TWidget, TCallback> : AndroidTextControl<TControl, TWidget, TCallback>, CommonControl.IHandler
		where TWidget: CommonControl
		where TControl: aw.TextView
		where TCallback: CommonControl.ICallback
	{
		public override av.View ContainerControl
		{
			get { return Control; }
		}
	}
}
