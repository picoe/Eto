using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
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

		public void AddTab (TabPage page)
		{
			Control.Add ((NSTabViewItem)page.ControlObject);
		}

		public void RemoveTab (TabPage page)
		{
			Control.Remove ((NSTabViewItem)page.ControlObject);
		}

		#endregion
	}
}
