using Eto.Forms;
using Eto.Drawing;
//using Eto.Platform.Xaml.Drawing;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using System;
using Eto.Platform.Direct2D.Drawing;

namespace Eto.Platform.Xaml.Forms
{
	/// <summary>
	/// Common control handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfCommonControl<TControl, TWidget> : WpfFrameworkElement<TControl, TWidget>, IControl
		where TControl : sw.FrameworkElement
		where TWidget : Control
	{
		Font font;
		public Font Font
		{
			get
			{
#if TODO_XAML
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Widget.Generator, Control));
				return font;
#else
				throw new NotImplementedException();
#endif
			}
			set
			{
				font = value;
#if TODO_XAML
				FontHandler.Apply (Control, SetDecorations, font);
#else
				//throw new NotImplementedException();
#endif
			}
		}
	}

	public class WpfControl<TControl, TWidget> : WpfCommonControl<TControl, TWidget>, IControl
		where TControl : swc.Control
		where TWidget: Control
	{
		public override Color BackgroundColor
		{
			get { return (ContainerControl as swc.Control ?? Control).Background.ToEtoColor(); }
			set { (ContainerControl as swc.Control ?? Control).Background = value.ToWpfBrush(Control.Background); }
		}

#if TODO_XAML
		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}
#endif
	}
}
