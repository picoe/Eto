using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public class WpfControl<TControl, TWidget, TCallback> : WpfFrameworkElement<TControl, TWidget, TCallback>, Control.IHandler
		where TControl : swc.Control
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override Color BackgroundColor
		{
			get { return (ContainerControl as swc.Control ?? Control).Background.ToEtoColor(); }
			set { (ContainerControl as swc.Control ?? Control).Background = value.ToWpfBrush(Control.Background); }
		}

		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}

		static readonly object FontKey = new object();

		public Font Font
		{
			get { return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control))); }
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					Widget.Properties[FontKey] = value;
					FontHandler.Apply(Control, SetDecorations, value);
				}
			}
		}

	}
}
