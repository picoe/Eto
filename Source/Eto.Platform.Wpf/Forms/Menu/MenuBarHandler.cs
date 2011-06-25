using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class MenuBarHandler : WidgetHandler<System.Windows.Controls.Menu, MenuBar>, IMenuBar
	{
		public MenuBarHandler ()
		{
			Control = new System.Windows.Controls.Menu ();

		}

		public void AddMenu (int index, MenuItem item)
		{
			
		}

		public void RemoveMenu (MenuItem item)
		{

			
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}
	}
}
