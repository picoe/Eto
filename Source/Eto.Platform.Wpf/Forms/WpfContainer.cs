using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfContainer<T, W> : WpfFrameworkElement<T, W>, IContainer
		where T: sw.FrameworkElement
		where W: Container
	{

		public override sw.Size PreferredSize
		{
			get
			{
				var size = sw.Size.Empty;
				var handler = this.Widget.Layout.Handler as IWpfLayout;
				if (handler != null)
					size = handler.PreferredSize;
				var baseSize = base.PreferredSize;
				return new sw.Size (Math.Max (size.Width, baseSize.Width), Math.Max (size.Height, baseSize.Height));
			}
		}

		public abstract Eto.Drawing.Size ClientSize { get; set; }

		public abstract object ContainerObject { get; }

		public abstract void SetLayout (Layout layout);

		public abstract Size? MinimumSize { get; set; }
	}
}
