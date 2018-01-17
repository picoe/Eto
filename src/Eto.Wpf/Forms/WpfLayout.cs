using Eto.Forms;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public abstract class WpfLayout<TControl, TWidget, TCallback> : WpfContainer<TControl, TWidget, TCallback>, Layout.IHandler
		where TControl : sw.FrameworkElement
		where TWidget : Layout
		where TCallback : Layout.ICallback
	{

		public virtual void Update()
		{
			Control.UpdateLayout();
		}
	}
}
