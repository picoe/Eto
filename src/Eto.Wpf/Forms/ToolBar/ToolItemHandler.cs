using Eto.Drawing;
using Eto.Forms;
using System;
using sw = System.Windows;

namespace Eto.Wpf.Forms.ToolBar
{
	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler
		where TControl : System.Windows.UIElement
		where TWidget : ToolItem
	{
		public abstract string Text { get; set; }

		public abstract string ToolTip { get; set; }

		public abstract Image Image { get; set; }

		public abstract bool Enabled { get; set; }
		public bool Visible
		{
			get => Control.Visibility == sw.Visibility.Visible;
			set => Control.Visibility = value ? sw.Visibility.Visible : sw.Visibility.Collapsed;
		}

		public virtual void CreateFromCommand(Command command)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}
	}
}