using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class ImageMenuItemHandler : WidgetHandler<System.Windows.Controls.MenuItem, ImageMenuItem>, IImageMenuItem
	{
		Key shortcut;
		Eto.Drawing.Icon icon;

		public ImageMenuItemHandler ()
		{
			Control = new System.Windows.Controls.MenuItem();
		}

		public Eto.Drawing.Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (icon != null)
					Control.Icon = icon.ControlObject;
				else
					Control.Icon = null;
			}
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Key Shortcut
		{
			get { return shortcut; }
			set { shortcut = value; }
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert (index, item.ControlObject);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove (item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();	
		}
	}
}
