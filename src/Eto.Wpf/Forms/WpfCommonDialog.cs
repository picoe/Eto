using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public abstract class WpfCommonDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, CommonDialog.IHandler
		where TControl : mw.CommonDialog
		where TWidget : CommonDialog
	{
		public virtual DialogResult ShowDialog (Window parent)
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
