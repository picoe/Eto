using Eto.Forms;
using Eto.Drawing;
using swm = System.Windows.Media;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class WpfPanel<T,W> : WpfFrameworkElement<T,W>, IControl
		where T : sw.Controls.Panel
		where W: Control
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
