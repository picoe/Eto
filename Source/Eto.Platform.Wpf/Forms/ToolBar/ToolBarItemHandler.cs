using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class ToolBarItemHandler<T, W> : WidgetHandler<T, W>, IToolBarItem
		where T : System.Windows.UIElement
		where W : ToolBarItem
	{
	}
}
