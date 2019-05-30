using Eto.Drawing;
using Eto.Forms;
using System;
using Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler
		where TControl : UIElement
		where TWidget : ToolItem
	{
		public abstract string Text { get; set; }

		public abstract string ToolTip { get; set; }

		public abstract Image Image { get; set; }

		public abstract bool Enabled { get; set; }

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
