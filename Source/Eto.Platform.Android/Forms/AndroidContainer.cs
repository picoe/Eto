using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms
{
	/// <summary>
	/// Base handler for <see cref="IContainer"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidContainer<T, TWidget> : AndroidControl<T, TWidget>, IContainer
		where TWidget: Container
	{
		protected AndroidContainer()
		{
		}

		public virtual bool RecurseToChildren { get { return true; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }
	}
}