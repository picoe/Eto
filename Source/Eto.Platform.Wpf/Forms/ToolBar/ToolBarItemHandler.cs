using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class ToolBarItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IToolBarItem
		where TControl : System.Windows.UIElement
		where TWidget : ToolBarItem
	{
	}
}
