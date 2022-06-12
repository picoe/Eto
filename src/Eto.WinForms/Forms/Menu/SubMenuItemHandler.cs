using swf = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Menu
{
	public class SubMenuItemHandler : ButtonMenuItemHandler<SubMenuItem, SubMenuItem.ICallback>, SubMenuItem.IHandler
	{
		swf.ToolStripMenuItem hiddenItem;

		public SubMenuItemHandler()
		{
			hiddenItem = new swf.ToolStripMenuItem();
			Control.DropDownItems.Add(hiddenItem);
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (hiddenItem != null)
			{
				Control.DropDownItems.Remove(hiddenItem);
				hiddenItem = null;
			}
			base.AddMenu(index, item);
		}

		public override void RemoveMenu(MenuItem item)
		{
			base.RemoveMenu(item);
			if (Control.DropDownItems.Count == 0)
			{
				hiddenItem = hiddenItem ?? new swf.ToolStripMenuItem();
				Control.DropDownItems.Add(hiddenItem);
			}
		}

		public override void Clear()
		{
			base.Clear();
			hiddenItem = hiddenItem ?? new swf.ToolStripMenuItem();
			Control.DropDownItems.Add(hiddenItem);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SubMenuItem.OpeningEvent:
					// handled with HandleDropDownOpened
					break;
				case SubMenuItem.ClosingEvent:
					HandleEvent(SubMenuItem.ClosingEvent);
					break;
				case SubMenuItem.ClosedEvent:
					Control.DropDownClosed += Control_DropDownClosed;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_DropDownClosed(object sender, EventArgs e)
		{
			// actually happens before the item is clicked
			Callback.OnClosing(Widget, EventArgs.Empty);
			Application.Instance.AsyncInvoke(() => Callback.OnClosed(Widget, EventArgs.Empty));
		}

		protected override void HandleDropDownOpened(object sender, EventArgs e)
		{
			Callback.OnOpening(Widget, EventArgs.Empty);

			base.HandleDropDownOpened(sender, e);
		}
	}
}
