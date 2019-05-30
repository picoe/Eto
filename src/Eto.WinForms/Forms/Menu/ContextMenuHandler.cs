using System;
using System.Linq;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Menu
{
	public class ContextMenuHandler : WidgetHandler<swf.ContextMenuStrip, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{

		public ContextMenuHandler()
		{
			Control = new swf.ContextMenuStrip();
			Control.Opening += HandleOpening;
			Control.KeyDown += HandleKeyDown;
		}

		void HandleKeyDown(object sender, swf.KeyEventArgs e)
		{
			var shortcut = e.KeyData.ToEto();
			var item = Widget.GetChildren().FirstOrDefault(r => r.Shortcut == shortcut);
			if (item != null)
			{
				Control.Close();
				item.PerformClick();
				e.Handled = true;
			}
		}

		void HandleOpening(object sender, EventArgs e)
		{
			foreach (var item in Widget.Items)
			{
				var callback = ((ICallbackSource)item).Callback as MenuItem.ICallback;
				if (callback != null)
					callback.OnValidate(item, e);
			}

			Callback.OnOpening(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
					// handled intrinsically
					break;
				case ContextMenu.ClosingEvent:
					HandleEvent(ContextMenu.ClosedEvent);
					break;
				case ContextMenu.ClosedEvent:
					Control.Closed += (sender, e) =>
					{
						// actually happens before the item is clicked
						Callback.OnClosing(Widget, EventArgs.Empty);
						Application.Instance.AsyncInvoke(() => Callback.OnClosed(Widget, EventArgs.Empty));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
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

		public void Show(Control relativeTo, PointF? location)
		{
			if (relativeTo != null)
			{
				var control = relativeTo.GetContainerControl();
				var position = location?.ToSDPoint() ?? control.PointToClient(swf.Control.MousePosition);
				Control.Show(control, position.X, position.Y);
			}
			else
			{
				var position = location?.ToSDPoint() ?? swf.Control.MousePosition;
				Control.Show(position);
			}
		}
	}
}
