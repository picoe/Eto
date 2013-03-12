using System;
using Eto.Forms;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public interface IWindowsLayout
	{
		object LayoutObject { get; }

		Size DesiredSize { get; }

		void SetScale (bool xscale, bool yscale);
	}
	
	public abstract class WindowsLayout<T, W> : WidgetHandler<T, W>, ILayout, IWindowsLayout
		where W: Layout
	{
		protected bool XScale { get; set; }
		protected bool YScale { get; set; }

		public abstract Size DesiredSize { get; }

		public WindowsLayout ()
		{
			XScale = YScale = true;
		}

		public virtual void SetScale (bool xscale, bool yscale)
		{
			this.XScale = xscale;
			this.YScale = yscale;
		}

		public virtual object LayoutObject {
			get { return null; }
		}
		
		public virtual void OnPreLoad ()
		{
		}

		public virtual void OnLoad ()
		{
		}

		public virtual void OnLoadComplete ()
		{
		}

		public virtual void OnUnLoad ()
		{
		}

		public virtual void Update ()
		{
		}

		public virtual void AttachedToContainer ()
		{
		}
	}
}
