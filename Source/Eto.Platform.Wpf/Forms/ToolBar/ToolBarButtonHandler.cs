using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class ToolBarButtonHandler : ToolBarItemHandler<System.Windows.Controls.Button, ToolBarButton>, IToolBarButton
	{
		public ToolBarButtonHandler ()
		{
			Control = new System.Windows.Controls.Button ();
		}

		public string Text
		{
			get { return Control.Content as string;	}
			set { Control.Content = value; }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Eto.Drawing.Icon Icon
		{
			get
			{
				return null;
			}
			set
			{
				
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}
	}
}
