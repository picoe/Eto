using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms
{
	public interface IWpfContainer
	{
		void Remove(sw.FrameworkElement child);

		void UpdatePreferredSize();
	}

	/// <summary>
	/// IContainer handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfContainer<TControl, TWidget, TCallback> : WpfFrameworkElement<TControl, TWidget, TCallback>, Container.IHandler, IWpfContainer
		where TControl : sw.FrameworkElement
		where TWidget : Container
		where TCallback: Container.ICallback
	{
		Size minimumSize;

		public bool RecurseToChildren { get { return true; } }

		protected override wf.Size DefaultSize => minimumSize.ToWpf();

		public abstract void Remove(sw.FrameworkElement child);

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public virtual Size MinimumSize
		{
			get { return minimumSize; }
			set
			{
				minimumSize = value;
				Size = value; // hack for now
				SetSize();
			}
		}

		public virtual void UpdatePreferredSize()
		{
			var parent = Widget.VisualParent.GetWpfContainer();
			if (parent != null)
				parent.UpdatePreferredSize();
		}

		public override void Invalidate(bool invalidateChildren)
		{
			base.Invalidate(invalidateChildren);
			if (invalidateChildren)
			{
				foreach (var control in Widget.VisualControls)
				{
					control.Invalidate(invalidateChildren);
				}
			}
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			base.Invalidate(rect, invalidateChildren);
			if (invalidateChildren)
			{
				foreach (var control in Widget.VisualControls)
				{
					control.Invalidate(rect, invalidateChildren);
				}
			}
		}
	}
}
