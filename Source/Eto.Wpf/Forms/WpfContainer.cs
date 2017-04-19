using swc = System.Windows.Controls;
using sw = System.Windows;
using swi = System.Windows.Input;
using Eto.Forms;
using Eto.Drawing;
using System;
using System.Collections.Generic;

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

		public override sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure)
		{
			var size = base.MeasureOverride(constraint, measure);
			size = size.Max(minimumSize.ToWpf());
			return size;
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

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			// group TabIndex by logical containers, not by visual ones.
			if (!Widget.IsVisualControl)
				swi.KeyboardNavigation.SetTabNavigation(ContainerControl, swi.KeyboardNavigationMode.Local);
		}

		public override IEnumerable<Control> VisualControls => Widget.Controls;
	}
}
