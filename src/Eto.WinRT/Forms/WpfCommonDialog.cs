#if TODO_XAML
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	public abstract class WpfCommonDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ICommonDialog
		where TControl : mw.CommonDialog
		where TWidget : CommonDialog
	{
		public DialogResult ShowDialog (Window parent)
		{
			bool? result;
			if (parent != null) {
				var owner = parent.ControlObject as sw.Window;
				result = Control.ShowDialog (owner);
			}
			else {
				result = Control.ShowDialog ();
			}
			return result != null && result.Value ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}
#endif