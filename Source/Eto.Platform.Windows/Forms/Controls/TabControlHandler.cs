using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TabControlHandler : WindowsControl<SWF.TabControl, TabControl>, ITabControl
	{
		public TabControlHandler ()
		{
			this.Control = new SWF.TabControl ();
			this.Control.ImageList = new SWF.ImageList{ ColorDepth = SWF.ColorDepth.Depth32Bit };
			this.Control.SelectedIndexChanged += control_SelectedIndexChanged;
		}

		private void control_SelectedIndexChanged (object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged (e);
		}
		#region ITabControl Members

		public int SelectedIndex {
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}
		
		public void InsertTab (int index, TabPage page)
		{
			var pageHandler = (TabPageHandler)page.Handler;
			if (index == -1 || index == this.Control.TabPages.Count)
				this.Control.TabPages.Add (pageHandler.Control);
			else
				this.Control.TabPages.Insert (index, pageHandler.Control);
		}
		
		public void RemoveTab (int index, TabPage page)
		{
			this.Control.TabPages.RemoveAt (index);
		}
		
		public void ClearTabs ()
		{
			this.Control.TabPages.Clear ();
		}

		#endregion
	}
}
