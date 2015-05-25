using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
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

		protected override Size DefaultSize { get { return minimumSize; } }

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
			var parent = Widget.Parent.GetWpfContainer();
			if (parent != null)
				parent.UpdatePreferredSize();
		}

		public override void Invalidate()
		{
			base.Invalidate();
			foreach (var control in Widget.Children)
			{
				control.Invalidate();
			}
		}

		public override void Invalidate(Rectangle rect)
		{
			base.Invalidate(rect);
			foreach (var control in Widget.Children)
			{
				control.Invalidate(rect);
			}
		}
	}
}
