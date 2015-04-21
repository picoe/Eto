using System;
using Eto.Drawing;
using Eto.Forms;
using Windows.UI.Xaml;

namespace Eto.WinRT.Forms.ToolBar
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
	}
}
