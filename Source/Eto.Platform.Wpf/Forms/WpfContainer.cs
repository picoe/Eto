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
	public interface IWpfContainer
	{
		void Remove(sw.FrameworkElement child);
	}

	public abstract class WpfContainer<T, W> : WpfFrameworkElement<T, W>, IContainer, IWpfContainer
		where T: sw.FrameworkElement
		where W: Container
	{
		public abstract void Remove(sw.FrameworkElement child);

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public virtual Size MinimumSize
		{
			get
			{
				return new Size((int)Control.MinWidth, (int)Control.MinHeight);
			}
			set
			{
				Control.MinWidth = value.Width;
				Control.MinHeight = value.Height;
			}
		}

		public override void Invalidate ()
		{
			base.Invalidate ();
			foreach (var control in Widget.Children) {
				control.Invalidate ();
			}
		}

		public override void Invalidate (Rectangle rect)
		{
			base.Invalidate (rect);
			foreach (var control in Widget.Children) {
				control.Invalidate (rect);
			}
		}
	}
}
