using Eto.Forms;
using Eto.Drawing;
//using Eto.WinRT.Drawing;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using System;
using Eto.Direct2D.Drawing;

namespace Eto.WinRT.Forms
{
	/// <summary>
	/// Common control handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfCommonControl<TControl, TWidget, TCallback> : WpfFrameworkElement<TControl, TWidget, TCallback>, Control.IHandler
		where TControl : sw.FrameworkElement
		where TWidget : Control
		where TCallback : Control.ICallback
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

	public class WpfControl<TControl, TWidget, TCallback> : WpfCommonControl<TControl, TWidget, TCallback>, Control.IHandler
		where TControl : swc.Control
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override Color BackgroundColor
		{
			get { return (ContainerControl as swc.Control ?? Control).Background.ToEtoColor(); }
			set { (ContainerControl as swc.Control ?? Control).Background = value.ToWpfBrush(Control.Background); }
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}

#if TODO_XAML
		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}
#endif
		public virtual bool ShowBorder
		{
			get { return true; }
			set { }
		}
	}
}
