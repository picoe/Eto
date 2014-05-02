using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public interface IWpfContainer
	{
		void Remove(sw.FrameworkElement child);

		void UpdatePreferredSize();
	}

	public abstract class WpfContainer<TControl, TWidget> : WpfFrameworkElement<TControl, TWidget>, IContainer, IWpfContainer
		where TControl : sw.FrameworkElement
		where TWidget : Container
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
