using System;
using System.Linq;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ContextMenuHandler : WidgetHandler<swf.ContextMenuStrip, ContextMenu>, IContextMenu
	{

		public ContextMenuHandler()
		{
			this.Control = new System.Windows.Forms.ContextMenuStrip();
			this.Control.Opened += HandleOpened;
		}

		void HandleOpened(object sender, EventArgs e)
		{
			foreach (var item in Widget.Items)
			{
				item.OnValidate(EventArgs.Empty);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Items.Insert(index, (swf.ToolStripItem)item.ControlObject);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Items.Remove((swf.ToolStripItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.Items.Clear();
		}

		public void Show(Control relativeTo)
		{
			var position = swf.Control.MousePosition;
			if (relativeTo != null)
			{
				var control = relativeTo.GetContainerControl();
				position = control.PointToClient(position);
				Control.Show(control, position.X, position.Y);
			}
			else
				Control.Show(position);
		}
	}
}
