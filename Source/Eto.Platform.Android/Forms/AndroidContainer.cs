using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms
{
	public abstract class AndroidContainer<T, TWidget> : AndroidControl<T, TWidget>, IContainer
		where TWidget: Container
	{
		protected AndroidContainer()
		{
		}

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }
	}
}

