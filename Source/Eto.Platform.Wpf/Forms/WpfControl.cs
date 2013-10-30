using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class WpfControl<T, W> : WpfFrameworkElement<T, W>, IControl
		where T : swc.Control
		where W: Control
	{
		Font font;

		public override Color BackgroundColor
		{
			get { return (ContainerControl as swc.Control ?? Control).Background.ToEtoColor(); }
			set { (ContainerControl as swc.Control ?? Control).Background = value.ToWpfBrush(Control.Background); }
		}

		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}

		public Font Font
		{
			get
			{
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Widget.Generator, Control));
				return font;
			}
			set
			{
				font = value;
				FontHandler.Apply (Control, SetDecorations, font);
			}
		}

	}
}
