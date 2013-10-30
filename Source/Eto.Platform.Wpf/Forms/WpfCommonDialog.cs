using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfCommonDialog<T, W> : WidgetHandler<T, W>, ICommonDialog
		where T : mw.CommonDialog
		where W : CommonDialog
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
