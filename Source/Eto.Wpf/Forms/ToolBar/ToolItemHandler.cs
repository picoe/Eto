using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.Wpf.Forms
{
	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IToolItem
		where TControl : System.Windows.UIElement
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
