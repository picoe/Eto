using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TabControlHandler : MacView<NSTabView, TabControl>, ITabControl
	{
		public class EtoTabView : NSTabView, IMacControl
		{
			public object Handler { get; set; }
			
		}
		
		public TabControlHandler ()
		{
			Enabled = true;
			Control = new EtoTabView { Handler = this };
			Control.DidSelect += delegate {
				this.Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
		}

		#region ITabControl Members

		public int SelectedIndex {
			get { return Control.IndexOf (Control.Selected); }
			set { Control.SelectAt (value); }
		}
		
		public override bool Enabled {
			get; set;
		}
		
		public void InsertTab (int index, TabPage page)
		{
			if (index == -1)
				Control.Add (((TabPageHandler)page.Handler).Control);
			else
				Control.Insert (((TabPageHandler)page.Handler).Control, index);
		}
		
		public void ClearTabs ()
		{
			foreach (var tab in Control.Items)
				Control.Remove (tab);
		}
		
		public void RemoveTab (int index, TabPage page)
		{
			Control.Remove (((TabPageHandler)page.Handler).Control);
		}
		
		#endregion
	}
}
