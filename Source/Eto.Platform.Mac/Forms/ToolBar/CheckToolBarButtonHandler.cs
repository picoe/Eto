using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{

	public class CheckToolBarButtonHandler : ToolBarItemHandler<NSToolbarItem, CheckToolBarButton>, ICheckToolBarButton
	{
		bool isChecked;
		public CheckToolBarButtonHandler()
		{
		}

		#region ICheckToolBarButton Members


		public bool Checked
		{
			get { return isChecked; }
			set { 
				isChecked = value;
				if (isChecked && Control != null && Control.Toolbar != null) Control.Toolbar.SelectedItemIdentifier = Widget.ID;
			}
		}

		#endregion
		
		public override void CreateControl ()
		{
			base.CreateControl ();
			//if (isChecked && Control.) Control.Toolbar.SelectedItemIdentifier = Widget.ID;
		}
		
		public override void ControlAdded (ToolBarHandler toolbar)
		{
			base.ControlAdded (toolbar);
			if (isChecked) toolbar.Control.SelectedItemIdentifier = Widget.ID;
		}
		
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
		
	}


}
