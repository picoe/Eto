using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public interface IWpfLayout
	{
		void AutoSize ();
		sw.Size PreferredSize { get; }
	}

	public abstract class WpfLayout<T, W> : WidgetHandler<T, W>, ILayout, IWpfLayout
		where T: System.Windows.FrameworkElement
		where W: Layout
	{
		public abstract sw.Size PreferredSize { get; }

		public virtual void AutoSize ()
		{
		}

		public virtual void OnLoad ()
		{
		}

		public virtual void OnLoadComplete ()
		{
		}

		public virtual void Update ()
		{
		}

		public virtual void OnPreLoad ()
		{
		}

		public virtual void AttachedToContainer ()
		{
		}
	}
}
