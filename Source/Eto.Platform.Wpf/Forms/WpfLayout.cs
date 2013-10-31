using Eto.Forms;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfLayout<TControl, TWidget> : WpfContainer<TControl, TWidget>, ILayout
		where TControl: sw.FrameworkElement
		where TWidget: Layout
	{

		public virtual void Update()
		{
			Control.UpdateLayout();
		}
	}
}
