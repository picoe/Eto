using System;
using System.Linq;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ImageMenuItemHandler : MenuHandler<SWF.ToolStripMenuItem, ImageMenuItem>, IImageMenuItem
	{
        Image image;
		int imageSize = 16;
		bool openedHandled;

		public ImageMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += (sender, e) => Widget.OnClick(EventArgs.Empty);
		}

		void HandleDropDownOpened (object sender, EventArgs e)
		{
			foreach (var item in Widget.MenuItems.OfType<MenuActionItem>()) {
				item.OnValidate (EventArgs.Empty);
			}
		}

		public int ImageSize
		{
			get { return imageSize; }
			set
			{
				imageSize = value;
				Control.Image = image.ToSD (imageSize);
			}
		}

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public string Text
		{
			get	{ return Control.Text; }
			set { Control.Text = value; }
		}
		
		public string ToolTip {
			get {
				return Control.ToolTipText;
			}
			set {
				Control.ToolTipText = value;
			}
		}

		public Keys Shortcut
		{
			get { return Control.ShortcutKeys.ToEto (); }
			set 
			{
				var key = value.ToSWF ();
				if (SWF.ToolStripManager.IsValidShortcut(key)) Control.ShortcutKeys = key;
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToSD (imageSize);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.DropDownItems.Insert(index, (SWF.ToolStripItem)item.ControlObject);
			if (!openedHandled) {
				Control.DropDownOpening += HandleDropDownOpened;
				openedHandled = true;
			}
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.DropDownItems.Remove((SWF.ToolStripItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.DropDownItems.Clear();
		}
	}
}
