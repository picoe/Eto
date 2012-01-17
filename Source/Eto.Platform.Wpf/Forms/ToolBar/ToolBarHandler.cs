using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class ToolBarHandler : WidgetHandler<System.Windows.Controls.ToolBar, ToolBar>, IToolBar
	{
		public ToolBarHandler ()
		{
			Control = new System.Windows.Controls.ToolBar ();
				 
		}

		public void AddButton (ToolBarItem button)
		{
		}

		public void RemoveButton (ToolBarItem button)
		{
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				return ToolBarTextAlign.Right;
			}
			set
			{
			}
		}

		public ToolBarDock Dock
		{
			get
			{
				return ToolBarDock.Top;
			}
			set
			{
			}
		}
	}
}
