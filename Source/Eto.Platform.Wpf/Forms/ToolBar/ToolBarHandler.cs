using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class ToolBarHandler : WidgetHandler<swc.ToolBar, ToolBar>, IToolBar
	{
		public ToolBarHandler ()
		{
			Control = new swc.ToolBar ();
			swc.ToolBarTray.SetIsLocked (Control, true);
		}

		public void AddButton (ToolBarItem button)
		{
			Control.Items.Add (button.ControlObject);
		}

		public void RemoveButton (ToolBarItem button)
		{
			Control.Items.Remove (button.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				return ToolBarTextAlign.Underneath;
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
