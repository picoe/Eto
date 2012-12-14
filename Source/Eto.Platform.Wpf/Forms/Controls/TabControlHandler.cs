using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TabControlHandler : WpfControl<swc.TabControl, TabControl>, ITabControl
	{
		public TabControlHandler ()
		{
			Control = new swc.TabControl ();
			Control.Loaded += delegate {
				Control.SelectionChanged += delegate {
					Widget.OnSelectedIndexChanged (EventArgs.Empty);
				};
			};
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public void InsertTab (int index, TabPage page)
		{
			if (index == -1)
				Control.Items.Add (page.ControlObject);
			else
				Control.Items.Insert (index, page.ControlObject);
		}

		public void ClearTabs ()
		{
			Control.Items.Clear ();
		}

		public void RemoveTab (int index, TabPage page)
		{
			Control.Items.Remove (page.ControlObject);
		}
	}
}
