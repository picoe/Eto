using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public interface IWpfContainer
	{
		void Remove(sw.FrameworkElement child);

		void UpdatePreferredSize();
	}

	public abstract class WpfContainer<TControl, TWidget, TCallback> : WpfFrameworkElement<TControl, TWidget, TCallback>, Container.IHandler, IWpfContainer
		where TControl : sw.FrameworkElement
		where TWidget : Container
		where TCallback : Container.ICallback
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
				SetSize();
			}
		}

		public override void Invalidate()
		{
			base.Invalidate();
			foreach (var control in Widget.VisualChildren)
			{
				control.Invalidate();
			}
		}

		public override void Invalidate(Rectangle rect)
		{
			base.Invalidate(rect);
			foreach (var control in Widget.VisualChildren)
			{
				control.Invalidate(rect);
			}
		}
	}
}
