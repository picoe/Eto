using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfLayout<T, W> : WpfContainer<T, W>, ILayout
		where T: System.Windows.FrameworkElement
		where W: Layout
	{

		public virtual void Update()
		{
			Control.UpdateLayout();
		}
	}
}
