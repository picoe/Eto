using System;
using System.Linq;
using SD = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Menu
{
	public class ButtonMenuItemHandler : ButtonMenuItemHandler<ButtonMenuItem, ButtonMenuItem.ICallback>
	{
	}

	public class ButtonMenuItemHandler<TWidget, TCallback> : MenuItemHandler<swf.ToolStripMenuItem, TWidget, TCallback>, ButtonMenuItem.IHandler
		where TWidget: ButtonMenuItem
		where TCallback: ButtonMenuItem.ICallback
	{
		Image image;
		int imageSize = 16;

		public ButtonMenuItemHandler()
		{
			Control = new swf.ToolStripMenuItem();
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
			Control.DropDownOpening += HandleDropDownOpened;
		}

		protected virtual void HandleDropDownOpened(object sender, EventArgs e)
		{
			foreach (var item in Widget.Items)
			{
				var callback = ((ICallbackSource)item).Callback as MenuItem.ICallback;
				if (callback != null)
					callback.OnValidate(item, e);
			}
		}

		public int ImageSize
		{
			get { return imageSize; }
			set
			{
				imageSize = value;
				Control.Image = image.ToSD(imageSize);
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToSD(imageSize);
			}
		}

		public virtual void AddMenu(int index, MenuItem item)
		{
			Control.DropDownItems.Insert(index, (swf.ToolStripItem)item.ControlObject);
		}

		public virtual void RemoveMenu(MenuItem item)
		{
			Control.DropDownItems.Remove((swf.ToolStripItem)item.ControlObject);
		}

		public virtual void Clear()
		{
			Control.DropDownItems.Clear();
		}
	}
}
