using Eto.Forms;
using Eto.Drawing;
using swm = System.Windows.Media;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class WpfPanel<TControl,TWidget> : WpfFrameworkElement<TControl,TWidget>, IControl
		where TControl : sw.Controls.Panel
		where TWidget: Control
	{
		public override Color BackgroundColor
		{
			get
			{
				var brush = Control.Background as swm.SolidColorBrush;
                return brush != null ? brush.Color.ToEto() : Colors.Black;
			}
			set
			{
				Control.Background = new swm.SolidColorBrush (value.ToWpf ());
			}
		}
	}
}
